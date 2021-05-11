using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiModels;
using Contracts.Logger;
using Contracts.Repository;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserManagement.WebAPI.Extensions;

namespace UserManagement.WebAPI.Controllers {
	[Route("1.0/encryptedInfo")]
	[ApiController]
	[AllowAnonymous]
	public class EncryptedInfoController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;
		private readonly IOptions<DecryptionSettings> _decryptionSettings;

		public EncryptedInfoController(ILoggerManager logger, IRepositoryWrapper repository, 
			IOptions<DecryptionSettings> decryptionSettings) {
			_logger = logger;
			_coadaptService = repository;
			_decryptionSettings = decryptionSettings;
		}

		/// <summary>
		/// Receive a new encrypted message
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - There is no need to have a logged-in user for COADAPT platform to receive encrypted info
		/// </remarks>
		/// <param name="encryptedInfo"></param>
		[HttpPost]
		[ProducesResponseType(typeof(String), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(String), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(String), StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ReceiveEncryptedInfo([FromBody]String encryptedInfo) {
			if (encryptedInfo == null) {
				_logger.LogError("ReceiveEncryptedInfo: Encrypted string sent from client is null.");
				return BadRequest("Encrypted string is null");
			}
			await System.IO.File.WriteAllTextAsync ("encr/encrypted.tmp", encryptedInfo);
			System.Diagnostics.ProcessStartInfo start =
				new System.Diagnostics.ProcessStartInfo {
					FileName = _decryptionSettings.Value.PythonExecutable,
					Arguments = $"{_decryptionSettings.Value.DecryptionScript} " +
					            $"{_decryptionSettings.Value.TempFolder} " +
					            $"{_decryptionSettings.Value.SenderPublicKeyPath} " +
					            $"{_decryptionSettings.Value.RecipientPrivateKeyPath}",
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};
			string result;
			try {
				using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start)) {
					if (process == null) {
						return StatusCode(StatusCodes.Status500InternalServerError, "Cannot execute system process");
					}
					using (StreamReader reader = process.StandardOutput) {
						string stderr = await process.StandardError.ReadToEndAsync();
						if (stderr != "") {
							_logger.LogError("Message decryption failed:\n" + stderr);
							return StatusCode(StatusCodes.Status500InternalServerError, 
								"Message decryption failed:\n" + stderr);
						}
						result = await reader.ReadToEndAsync();
					}
				}
			}
			catch (Exception exception) {
				_logger.LogError("Data decoding script not executed:\n" + exception.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, 
					"Data decoding script not executed:\n" + exception.Message);
			}
			if (result.Length < 2 || result.Substring(0, 2) != "OK") {
				_logger.LogError("Message decryption did not complete successfully!");
				return StatusCode(StatusCodes.Status500InternalServerError, 
					"Message decryption did not complete successfully!");
			}
			result = await System.IO.File.ReadAllTextAsync($"{_decryptionSettings.Value.TempFolder}decrypted.tmp");
			try {
				var parseResult = await ParseDecodedMessage(result);
				if (parseResult != "OK") {
					_logger.LogWarn(parseResult);
					return BadRequest(parseResult);
				}
			}
			catch (Exception exception) {
				_logger.LogError("Decrypted data cannot be parsed:\n" + exception.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, 
					"Decrypted data cannot be parsed:\n" + exception.Message);
			}
			return Ok("Message decrypted successfully:\n" + result);
		}
		
		private async Task<string> ParseDecodedMessage(string message) {
			var data = JsonConvert.DeserializeObject<JObject>(message);
			data = (JObject)data.Properties().First().Value;
			var participantCode = (string)(data["Info"]?["Code"]);
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(participantCode);
			if (participant == null || participant.Id == 0) {
				return ("Participant " + participantCode + " does not exist");
			}
			_logger.LogInfo("Received info for participant " + participantCode);
			var gender = (string)(data["Info"]?["Gender"]);
			if (!string.IsNullOrEmpty(gender)) participant.Gender = gender;
			var language = (string)(data["Info"]?["Language"]);
			if (!string.IsNullOrEmpty(language)) participant.Language = language;
			var birthPlace = (string)(data["Info"]?["BirthPlace"]);
			if (!string.IsNullOrEmpty(birthPlace)) participant.BirthPlace = birthPlace;
			if (DateTime.TryParse((string)(data["Info"]?["DateOfBirth"]), out var dateOfBirth))
				participant.DateOfBirth = dateOfBirth;
			var files = (string)(data["Info"]?["Files"]);
			if (!string.IsNullOrEmpty(files)) participant.Files = files;
			var studies = (JArray)data["Info"]?["Trials"];
			if (studies != null) {
				for (var s = 0; s < studies.Count; s++) {
					var organizationName = (string)data["Info"]?["Trials"]?[s]?["Organization"];
					var organization = await _coadaptService.Organization.GetOrganizationByShortnameAsync(organizationName);
					if (organization == null || organization.Id == 0) {
						return ("Organization " + organizationName + " does not exist");
					}
					var studyName = (string)data["Info"]?["Trials"]?[s]?["Study"];
					var study = await _coadaptService.Study.GetStudyOfOrganizationByShortnameAsync(studyName, organization.Id);
					if (study == null || study.Id == 0) {
						return ("Study " + studyName + " of organization " + organizationName + " does not exist");
					}
					var siteName = (string)data["Info"]?["Trials"]?[s]?["Site"];
					var site = await _coadaptService.Site.GetSiteOfStudyByShortnameAsync(siteName, study.Id);
					if (site == null || site.Id == 0) {
						return ("Site " + siteName + " of study " + studyName + " does not exist");
					}
					var groupName = (string)data["Info"]?["Trials"]?[s]?["Group"];
					var group = await _coadaptService.Group.GetGroupOfStudyByShortnameAsync(groupName, study.Id);
					if (group == null || group.Id == 0) {
						return ("Group " + groupName + " of study " + studyName + " does not exist");
					}
					var studyParticipant = await _coadaptService.StudyParticipant
						.StudyParticipantByParticipantAndStudy(participant.Id, study.Id);
					if (studyParticipant == null || studyParticipant.StudyId == 0) {
						return (participantCode + " is not a participant of study " + studyName);
					}
					if (studyParticipant.SiteId != site.Id || studyParticipant.GroupId != group.Id) {
						return ("Participant " + participantCode + " of study " + studyName + 
						        " does not belong to site " + siteName + " or group " + groupName);
					}
					if (int.TryParse((string)(data["Info"]?["Trials"]?[s]?["DataCollectionTurn"]), out var dataCollectionTurn)) {
						studyParticipant.DataCollectionTurn = dataCollectionTurn;
					}
					studyParticipant.PlaceOfConsent = (string)(data["Info"]?["Trials"]?[s]?["PlaceOfConsent"]);
					studyParticipant.PlaceOfResidence = (string)(data["Info"]?["Trials"]?[s]?["PlaceOfResidence"]);
					studyParticipant.RegionOfResidence = (string)(data["Info"]?["Trials"]?[s]?["RegionOfResidence"]);
					studyParticipant.Education = (string)(data["Info"]?["Trials"]?[s]?["Education"]);
					var abandoned = (string)(data["Info"]?["Trials"]?[s]?["Abandoned"]);
					if (abandoned.ToLower() == "true" || abandoned.ToLower() == "t" ||
					    abandoned.ToLower() == "yes" || abandoned.ToLower() == "y") {
						studyParticipant.Abandoned = true;
					} else {
						studyParticipant.Abandoned = false;
					}
					var dateStr = (string)(data["Info"]?["Trials"]?[s]?["StartDate"]);
					if (DateTime.TryParse(dateStr, out var startDate)) {
						studyParticipant.StartDate = startDate;
					}
					dateStr = (string)(data["Info"]?["Trials"]?[s]?["EndDate"]);
					if (DateTime.TryParse(dateStr, out var endDate)) {
						studyParticipant.EndDate = endDate;
					}
					studyParticipant.MaritalStatus = (string)(data["Info"]?["Trials"]?[s]?["MaritalStatus"]);
					if (int.TryParse((string)(data["Info"]?["Trials"]?[s]?["NumberOfChildren"]), out var numberOfChildren)) {
						studyParticipant.NumberOfChildren = numberOfChildren;
					}
					studyParticipant.JobType = (string)(data["Info"]?["Trials"]?[s]?["JobType"]);
					dateStr = (string)(data["Info"]?["Trials"]?[s]?["DateOfCurrentJob"]);
					if (dateStr != null) {
						if (dateStr.Length == 4) dateStr += "-01-01";
						if (DateTime.TryParse(dateStr, out var dateOfCurrentJob)) {
							studyParticipant.DateOfCurrentJob = dateOfCurrentJob;
						}
					}
					_coadaptService.StudyParticipant.Update(studyParticipant);
					_logger.LogInfo("Taking part in study " + studyName + " (site " + siteName +
					                ", group " + groupName + ") of organization " + organizationName);
					dateStr = (string)(data["Info"]?["Trials"]?[s]?["DateOfFirstJob"]);
					if (dateStr != null) {
						if (dateStr.Length == 4) dateStr += "-01-01";
						if (DateTime.TryParse(dateStr, out var dateOfFirstJob)) {
							participant.DateOfFirstJob = dateOfFirstJob;
						}
					}
				}
				var psychologicalReportMessages = data["Info"]?["PsychologicalScores"]?.ToObject<List<PsychologicalReportMessage>>();
				foreach (var psychologicalReport in psychologicalReportMessages.Select(m => m.ToPsychologicalReport(participant.Id))) {
					var reportIdDb = await _coadaptService.PsychologicalReport
						.GetPsychologicalReportByParticipantIdAndDateAsync(participant.Id, psychologicalReport.DateOfReport);
					if (reportIdDb != null && reportIdDb.Id != 0) {
						_coadaptService.PsychologicalReport.UpdatePsychologicalReport(reportIdDb, psychologicalReport);
					} else {
						_coadaptService.PsychologicalReport.CreatePsychologicalReport((psychologicalReport));
					}
				}
				_coadaptService.Participant.Update(participant);
			}
			var signals = data["Signals"]?.ToObject<List<PhysiologicalSignalMessage>>();
			if (signals != null) {
				foreach (var physiologicalSignal in signals.Select(signal => signal.ToPhysiologicalSignal(participant.Id))) {
					var signalInDb = await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalByParticipantIdAndTypeAndDateAsync(
						participant.Id, physiologicalSignal.Type, physiologicalSignal.Timestamp);
					if (signalInDb != null && signalInDb.Id != 0) {
						_coadaptService.PhysiologicalSignal.UpdatePhysiologicalSignal(signalInDb, physiologicalSignal);
					} else {
						_coadaptService.PhysiologicalSignal.CreatePhysiologicalSignal(physiologicalSignal);
					}
				}
				_logger.LogInfo("Processed " + signals.Count + " signal samples");
			}
			var activities = data["OuraRing"]?["Activity"]?.ToObject<List<OuraActivityMessage>>();
			if (activities != null) {
				foreach (var ouraActivity in activities.Select(activity => activity.ToOuraActivity(participant.Id))) {
					var dbOuraActivity = await _coadaptService.OuraActivity.GetOuraActivityByParticipantIdAndDateAsync(
						participant.Id, ouraActivity.SummaryDate);
					if (dbOuraActivity != null && dbOuraActivity.Id != 0) {
						_coadaptService.OuraActivity.UpdateOuraActivity(dbOuraActivity, ouraActivity);
					} else {
						_coadaptService.OuraActivity.CreateOuraActivity(ouraActivity);
					}
					_logger.LogInfo("Received activity for " +
					                ouraActivity.SummaryDate.ToString(CultureInfo.InvariantCulture));
				}
			}
			var sleeps = data["OuraRing"]?["Sleep"]?.ToObject<List<OuraSleepMessage>>();
			if (sleeps != null) {
				foreach (var ouraSleep in sleeps.Select(sleep => sleep.ToOuraSleep(participant.Id)))
				{
					var dbOuraSleep = await _coadaptService.OuraSleep.GetOuraSleepByParticipantIdAndDateAsync(
						participant.Id, ouraSleep.SummaryDate);
					if (dbOuraSleep != null && dbOuraSleep.Id != 0) {
						_coadaptService.OuraSleep.UpdateOuraSleep(dbOuraSleep, ouraSleep);
					} else {
						_coadaptService.OuraSleep.CreateOuraSleep(ouraSleep);
					}
					_logger.LogInfo(
						"Received sleep for " + ouraSleep.SummaryDate.ToString(CultureInfo.InvariantCulture));
				}
			}
			var readinesses = (data["OuraRing"]?["Readiness"])?.ToObject<List<OuraReadiness>>();
			if (readinesses != null) {
				foreach (var ouraReadiness in readinesses) {
					ouraReadiness.ParticipantId = participant.Id;
					var dbOuraReadiness = await _coadaptService.OuraReadiness.GetOuraReadinessByParticipantIdAndDateAsync(
						participant.Id, ouraReadiness.SummaryDate);
					if (dbOuraReadiness != null && dbOuraReadiness.Id != 0) {
						_coadaptService.OuraReadiness.UpdateOuraReadiness(dbOuraReadiness, ouraReadiness);
					} else {
						_coadaptService.OuraReadiness.CreateOuraReadiness(ouraReadiness);
					}
					_logger.LogInfo("Received readiness for " +
					                ouraReadiness.SummaryDate.ToString(CultureInfo.InvariantCulture));
				}
			}
			await _coadaptService.SaveAsync();
			return ("OK");
		}
	}
}
