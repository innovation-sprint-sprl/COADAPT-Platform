using System.Linq;
using System.Threading.Tasks;
using Constants;
using Contracts.Logger;
using Contracts.Repository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiModels;
using System;
using System.Collections.Generic;

namespace UserManagement.WebAPI.Controllers {
	[Route("1.0/psychologicalInfo")]
	[ApiController]
	[Authorize(Policy = "Everyone")]
	public class PsychologicalInfoController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;

		public PsychologicalInfoController(ILoggerManager logger, IRepositoryWrapper repository) {
			_logger = logger;
			_coadaptService = repository;
		}

		/// <summary>
		/// Retrieve all personal information entries
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve personal information entries with the following restrictions:
		/// -- Administrators can retrieve personal information entries for any participant
		/// -- Sub-administrators can retrieve personal information entries for participants of studies in their organization
		/// -- Supervisors can retrieve personal information entries for participants of their studies
		/// -- Therapists can retrieve personal information entries for participants they monitor
		/// -- Participants can only retrieve personal information entries of themselves
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="fromDate"></param>
		[HttpGet]
		[ProducesResponseType(typeof(PersonalInformation), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllPersonalInformation([FromQuery(Name = "fromDate")] DateTime fromDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.ParticipantRole) {
				IUserDetails requestingUser = await _coadaptService.Participant.GetParticipantByUserIdAsync(userId);
				return fromDate == DateTime.MinValue ?
					Ok(await _coadaptService.PersonalInformation.GetAllPersonalInformationByParticipantIdAsync(requestingUser.Id)) :
					Ok(await _coadaptService.PersonalInformation.GetAllPersonalInformationAfterDateByParticipantIdAsync(fromDate, requestingUser.Id));
			}
			if (role == Role.AdministratorRole) {
				return fromDate == DateTime.MinValue ? 
					Ok(await _coadaptService.PersonalInformation.GetAllPersonalInformationAsync()) : 
					Ok(await _coadaptService.PersonalInformation.GetAllPersonalInformationAfterDateAsync(fromDate));
			}
			var participants = await _coadaptService.Participant.GetAllParticipantsAsync();
			var filteredPersonalInformations = new List<PersonalInformation>();
			foreach (var participant in participants) {
				if (role == Role.SubAdministratorRole) {
					if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredPersonalInformations.AddRange(await _coadaptService.PersonalInformation.GetAllPersonalInformationByParticipantIdAsync(participant.Id));
					} else {
						filteredPersonalInformations.AddRange(await _coadaptService.PersonalInformation.GetAllPersonalInformationAfterDateByParticipantIdAsync(fromDate, participant.Id));
					}
				} else if (role == Role.SupervisorRole) {
					if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredPersonalInformations.AddRange(await _coadaptService.PersonalInformation.GetAllPersonalInformationByParticipantIdAsync(participant.Id));
					} else {
						filteredPersonalInformations.AddRange(await _coadaptService.PersonalInformation.GetAllPersonalInformationAfterDateByParticipantIdAsync(fromDate, participant.Id));
					}
				} else if (role == Role.TherapistRole) {
					if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredPersonalInformations.AddRange(await _coadaptService.PersonalInformation.GetAllPersonalInformationByParticipantIdAsync(participant.Id));
					} else {
						filteredPersonalInformations.AddRange(await _coadaptService.PersonalInformation.GetAllPersonalInformationAfterDateByParticipantIdAsync(fromDate, participant.Id));
					}
				}
			}
			return Ok(filteredPersonalInformations);
		}

		/// <summary>
		/// Retrieve all personal information entries of a participant with given code
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve personal information entries of a given participant with the following restrictions:
		/// -- Administrators can retrieve personal information entries for any participant
		/// -- Sub-administrators can retrieve personal information entries for participants of studies in their organization
		/// -- Supervisors can retrieve personal information entries for participants of their studies
		/// -- Therapists can retrieve personal information entries for participants they monitor
		/// -- Participants can only retrieve personal information entries of themselves
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="fromDate"></param>
		[HttpGet("participant/{code}")]
		[ProducesResponseType(typeof(PersonalInformation), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllPersonalInformationOfParticipant(string code, [FromQuery(Name = "fromDate")] DateTime fromDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A sub-administrator can retrieve only personal information of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A supervisor can retrieve only personal information of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(participant.Id, userId)) {
					return BadRequest("A participant can retrieve only own personal information");
				}
			} else if (role == Role.TherapistRole) {
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only personal information of monitored participants");
				}
			}
			return fromDate == DateTime.MinValue ?
				Ok(await _coadaptService.PersonalInformation.GetAllPersonalInformationByParticipantIdAsync(participant.Id)) :
				Ok(await _coadaptService.PersonalInformation.GetAllPersonalInformationAfterDateByParticipantIdAsync(fromDate, participant.Id));
		}

		/// <summary>
		/// Retrieve the latest personal information entry that modified a given property for a participant with given code
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve personal information entries of a given participant with the following restrictions:
		/// -- Administrators can retrieve personal information entries for any participant
		/// -- Sub-administrators can retrieve personal information entries for participants of studies in their organization
		/// -- Supervisors can retrieve personal information entries for participants of their studies
		/// -- Therapists can retrieve personal information entries for participants they monitor
		/// -- Participants can only retrieve personal information entries of themselves
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="property"></param>
		[HttpGet("property/{property}/participant/{code}")]
		[ProducesResponseType(typeof(PersonalInformation), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetLatestPersonalInformationOfParticipantForProperty(string code, string property) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A sub-administrator can retrieve only personal information of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A supervisor can retrieve only personal information of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(participant.Id, userId)) {
					return BadRequest("A participant can retrieve only own personal information");
				}
			} else if (role == Role.TherapistRole) {
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only personal information of monitored participants");
				}
			}
			if (string.IsNullOrEmpty(property)) {
				return BadRequest("The property to search for must be provided");
			}
			if (typeof(PersonalInformation).GetProperty(property) == null) {
				return BadRequest($"The specified property '{property}' is invalid");
			}
			var personalInformation = await _coadaptService.PersonalInformation.GetLatestPersonalInformationByParticipantIdAndPropertyAsync(participant.Id, property);
			personalInformation.Participant = await _coadaptService.Participant.GetParticipantByIdAsync(personalInformation.ParticipantId);
			return Ok(personalInformation);
		}

		/// <summary>
		/// Retrieve all personal information entries that modified a given property for a participant with given code
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve personal information entries of a given participant with the following restrictions:
		/// -- Administrators can retrieve personal information entries for any participant
		/// -- Sub-administrators can retrieve personal information entries for participants of studies in their organization
		/// -- Supervisors can retrieve personal information entries for participants of their studies
		/// -- Therapists can retrieve personal information entries for participants they monitor
		/// -- Participants can only retrieve personal information entries of themselves
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="property"></param>
		/// <param name="fromDate"></param>
		[HttpGet("all/property/{property}/participant/{code}")]
		[ProducesResponseType(typeof(PersonalInformation), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllPersonalInformationOfParticipantForProperty(string code, string property, [FromQuery(Name = "fromDate")] DateTime fromDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A sub-administrator can retrieve only personal information of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A supervisor can retrieve only personal information of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(participant.Id, userId)) {
					return BadRequest("A participant can retrieve only own personal information");
				}
			} else if (role == Role.TherapistRole) {
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only personal information of monitored participants");
				}
			}
			if (string.IsNullOrEmpty(property)) {
				return BadRequest("The property to search for must be provided");
			}
			if (typeof(PersonalInformation).GetProperty(property) == null) {
				return BadRequest($"The specified property '{property}' is invalid");
			}
			return fromDate == DateTime.MinValue ?
				Ok(await _coadaptService.PersonalInformation.GetAllPersonalInformationByParticipantIdAndPropertyAsync(participant.Id, property)) :
				Ok(await _coadaptService.PersonalInformation.GetAllPersonalInformationAfterDateByParticipantIdAndPropertyAsync(fromDate, participant.Id, property));
		}

		/// <summary>
		///	Retrieve the personal information entry with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve a personal information entry by ID with the following restrictions:
		/// -- Administrators can retrieve a personal information entry for any participant
		/// -- Sub-administrators can retrieve a personal information entry for participants of studies in their organization
		/// -- Supervisors can retrieve a personal information entry for participants of their studies
		/// -- Therapists can retrieve personal information entries for participants they monitor
		/// -- Participants can only retrieve a personal information entry of themselves
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "PersonalInformationById")]
		[ProducesResponseType(typeof(PersonalInformation), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetPersonalInformationById(int id) {
			var personalInformation = await _coadaptService.PersonalInformation.GetPersonalInformationByIdAsync(id);
			if (personalInformation.IsEmptyObject()) {
				_logger.LogWarn($"GetPersonalInformationById: PersonalInformation with ID {id} not found!");
				return NotFound("PersonalInformation with requested ID does not exist");
			}
			personalInformation.Participant = await _coadaptService.Participant.GetParticipantByIdAsync(personalInformation.ParticipantId);
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(personalInformation.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can view only personal information of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(personalInformation.ParticipantId, userId)) {
					return BadRequest("A supervisor can view only personal information of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(personalInformation.ParticipantId, userId)) {
					return BadRequest("A participant can view only own personal information");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(personalInformation.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only personal information of monitored participants");
				}
			}
			return Ok(personalInformation);
		}

		/// <summary>
		/// Create a new personal information entry
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can create a new personal information entry with the following restrictions:
		/// -- Administrators can create a new personal information entry for any participant
		/// -- Sub-administrators only create a new personal information entry for a participant to sites of their organization
		/// -- Supervisors only create a new personal information entry for a participant  to sites of their studies
		/// -- Therapists can create a personal information entry for participants they monitor
		/// -- Participants can only create a new personal information entry for themselves
		/// - Any number of the information fields can be omitted, to report just the new information available
		/// </remarks>
		/// <param name="personalInformationRequest"></param>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreatePersonalInformation([FromBody]PersonalInformationRequest personalInformationRequest) {
			if (personalInformationRequest == null) {
				_logger.LogError("CreatePersonalInformation: PersonalInformationRequest object sent from client is null.");
				return BadRequest("PersonalInformationRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreatePersonalInformation: Invalid PersonalInformationRequest object sent from client.");
				return BadRequest("Invalid PersonalInformationRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(personalInformationRequest.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can create only personal information of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(personalInformationRequest.ParticipantId, userId)) {
					return BadRequest("A supervisor can create only personal information of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(personalInformationRequest.ParticipantId, userId)) {
					return BadRequest("A participant can create only own personal information");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(personalInformationRequest.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can create only personal information of monitored participants");
				}
			}
			var personalInformation = new PersonalInformation();
			personalInformation.FromRequest(personalInformationRequest);
			_coadaptService.PersonalInformation.CreatePersonalInformation(personalInformation);
			await _coadaptService.SaveAsync();
			return CreatedAtRoute("PersonalInformationById", new { id = personalInformation.Id }, personalInformation);
		}

		/// <summary>
		/// Delete the personal information entry with given id.
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can delete a new personal information entry with the following restrictions:
		/// -- Administrators can delete a personal information entry of any participant
		/// -- Sub-administrators can delete a personal information entry of participants of studies in their organization
		/// -- Supervisors can delete a personal information entry of participants of their studies
		/// -- Therapists can delete a personal information entry of participants they monitor
		/// -- Participants can only delete a personal information entry of themselves
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeletePersonalInformation(int id) {
			var personalInformation = await _coadaptService.PersonalInformation.GetPersonalInformationByIdAsync(id);
			if (personalInformation.IsEmptyObject()) {
				_logger.LogDebug($"DeletePersonalInformation: PersonalInformation with id {id} not found.");
				return NotFound("PersonalInformation with requested id does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(personalInformation.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can delete only personal information of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(personalInformation.ParticipantId, userId)) {
					return BadRequest("A supervisor can delete only personal information of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(personalInformation.ParticipantId, userId)) {
					return BadRequest("A participant can delete only own personal information");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(personalInformation.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can delete only personal information of monitored participants");
				}
			}
			_coadaptService.PersonalInformation.DeletePersonalInformation(personalInformation);
			await _coadaptService.SaveAsync();
			return NoContent();
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
			var studies = (ICollection<Study>) await StudiesIdOfUserIdAsync(userId);
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

		private async Task<bool> ParticipantSameAsUserIdAsync(int participantId, string userId) {
			var participant = await _coadaptService.Participant.GetParticipantByUserIdAsync(userId);
			return participant.Id == participantId;
		}

		private async Task<bool> ParticipantMonitoredByTherapistOfUserIdAsync(Participant participant, string userId) {
			var therapist = await _coadaptService.Therapist.GetTherapistByUserIdAsync(userId);
			return participant.TherapistId == therapist.Id;
		}

	}
}
