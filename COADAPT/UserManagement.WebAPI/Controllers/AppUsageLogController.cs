using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiModels;
using Constants;
using Contracts.Logger;
using Contracts.Repository;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.WebAPI.Controllers {
	[Route("1.0/appUsageLog")]
	[ApiController]
	[Authorize(Policy = "Everyone")]
	public class AppUsageLogController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;

		public AppUsageLogController(ILoggerManager logger, IRepositoryWrapper repository) {
			_logger = logger;
			_coadaptService = repository;
		}

		/// <summary>
		/// Retrieve all application usage log entries
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All Administrators, sub-administrators and supervisors can retrieve application usage log entries, with the following restrictions:
		/// -- Administrators can retrieve application usage log entries for any user
		/// -- Sub-administrators can retrieve application usage log entries for users in their organization
		/// -- Supervisors can retrieve application usage log entries for users of their studies
		/// -- Note that a therapist is considered to belong to a study if assigned to any participant of that study
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="fromDate"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpGet]
		[ProducesResponseType(typeof(AppUsageLog), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllAppUsageLogs([FromQuery(Name = "fromDate")] DateTime fromDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.AdministratorRole) {
				return fromDate == DateTime.MinValue ?
					Ok(await _coadaptService.AppUsageLog.GetAllAppUsageLogAsync()) :
					Ok(await _coadaptService.AppUsageLog.GetAllAppUsageLogAfterDateAsync(fromDate));
			}
			var participants = await _coadaptService.Participant.GetAllParticipantsAsync();
			var subAdmins = new List<SubAdministrator>();
			var supervisors = new List<Supervisor>();
			if (role == Role.SubAdministratorRole) {
				subAdmins.Add(await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId));
				supervisors = (List<Supervisor>) (await _coadaptService.Supervisor.GetAllSupervisorsAsync());
			} else {
				supervisors.Add(await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId));
			}
			var appUsageLogs = new List<AppUsageLog>();
			foreach (var subAdmin in subAdmins) {
				appUsageLogs.AddRange(await _coadaptService.AppUsageLog.GetAllAppUsageLogByUserIdAsync(subAdmin.Id));
			}
			foreach (var supervisor in supervisors) {
				appUsageLogs.AddRange(await _coadaptService.AppUsageLog.GetAllAppUsageLogByUserIdAsync(supervisor.Id));
			}
			var therapistIds = new List<int>();
			foreach (var participant in participants) {
				if (role == Role.SubAdministratorRole) {
					if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (participant.TherapistId != null && !therapistIds.Contains(participant.TherapistId.Value)) {
						therapistIds.Add(participant.TherapistId.Value);
					}
					if (fromDate == DateTime.MinValue) {
						appUsageLogs.AddRange(await _coadaptService.AppUsageLog.GetAllAppUsageLogByUserIdAsync(participant.Id));
					} else {
						appUsageLogs.AddRange(await _coadaptService.AppUsageLog.GetAllAppUsageLogAfterDateByUserIdAsync(fromDate, participant.Id));
					}
				} else if (role == Role.SupervisorRole) {
					if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (participant.TherapistId != null && !therapistIds.Contains(participant.TherapistId.Value)) {
						therapistIds.Add(participant.TherapistId.Value);
					}
					if (fromDate == DateTime.MinValue) {
						appUsageLogs.AddRange(await _coadaptService.AppUsageLog.GetAllAppUsageLogByUserIdAsync(participant.Id));
					} else {
						appUsageLogs.AddRange(await _coadaptService.AppUsageLog.GetAllAppUsageLogAfterDateByUserIdAsync(fromDate, participant.Id));
					}
				}
			}
			foreach (var id in therapistIds) {
				if (fromDate == DateTime.MinValue) {
					appUsageLogs.AddRange(await _coadaptService.AppUsageLog.GetAllAppUsageLogByUserIdAsync(id));
				} else {
					appUsageLogs.AddRange(await _coadaptService.AppUsageLog.GetAllAppUsageLogAfterDateByUserIdAsync(fromDate, id));
				}
			}
			return Ok(appUsageLogs);
		}



		/// <summary>
		/// Create a new application usage log entry
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - The applications can create a new application usage log when any user is logged in
		/// </remarks>
		/// <param name="appUsageLogRequest"></param>
		[HttpPost]
		[ProducesResponseType(typeof(AppUsageLog), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateAppUsageLog([FromBody] AppUsageLogRequest appUsageLogRequest) {
			if (appUsageLogRequest == null) {
				_logger.LogError("CreateAppUsageLog: AppUsageLogRequest object sent from client is null.");
				return BadRequest("AppUsageLogRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateAppUsageLog: Invalid AppUsageLogRequest object sent from client.");
				return BadRequest("Invalid AppUsageLogRequest object");
			}
			var appUsageLog = new AppUsageLog() {Message = appUsageLogRequest.Message, Tag =  appUsageLogRequest.Tag,
				UserId = appUsageLogRequest.UserId, ReportedOn = DateTime.Now};
			_coadaptService.AppUsageLog.CreateAppUsageLog(appUsageLog);
			await _coadaptService.SaveAsync();
			return Ok(appUsageLog);
		}

		private async Task<bool> ParticipantInOrganizationAsync(int participantId, int organizationId) {
			var studyParticipants = (ICollection<StudyParticipant>)await _coadaptService.StudyParticipant.StudyParticipantsByParticipant(participantId);
			foreach (var studyParticipant in studyParticipants) {
				var study = await _coadaptService.Study.GetStudyByIdAsync(studyParticipant.StudyId);
				if (study.OrganizationId == organizationId) {
					return true;
				}
			}
			return false;
		}

		private async Task<bool> ParticipantInStudyAsync(int participantId, int studyId) {
			var studyParticipants = (ICollection<StudyParticipant>)await _coadaptService.StudyParticipant.StudyParticipantsByParticipant(participantId);
			return studyParticipants.Any(studyParticipant => studyParticipant.StudyId == studyId);
		}

		private async Task<int> OrganizationIdOfUserIdAsync(string userId) {
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			if (subAdministrator == null || subAdministrator.Id == 0) {
				return -1;
			}
			var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
			return organization.Id;
		}

		private async Task<bool> ParticipantInStudiesOfUserIdAsync(int participantId, string userId) {
			var studies = (ICollection<Study>)await StudiesIdOfUserIdAsync(userId);
			foreach (var study in studies) {
				if (await ParticipantInStudyAsync(participantId, study.Id)) {
					return true;
				}
			}
			return false;
		}

		private async Task<bool> ParticipantInOrganizationOfUserIdAsync(int participantId, string userId) {
			return await ParticipantInOrganizationAsync(participantId, await OrganizationIdOfUserIdAsync(userId));
		}

		private async Task<IEnumerable<Study>> StudiesIdOfUserIdAsync(string userId) {
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (supervisor == null || supervisor.Id == 0) {
				return null;
			}
			return await _coadaptService.Study.GetStudiesBySupervisorIdAsync(supervisor.Id);
		}

	}
}
