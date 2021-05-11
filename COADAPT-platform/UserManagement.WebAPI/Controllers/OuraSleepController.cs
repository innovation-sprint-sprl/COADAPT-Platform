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
	[Route("1.0/ouraSleep")]
	[ApiController]
	[Authorize(Policy = "Everyone")]
	public class OuraSleepController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;

		public OuraSleepController(ILoggerManager logger, IRepositoryWrapper repository) {
			_logger = logger;
			_coadaptService = repository;
		}

		/// <summary>
		/// Retrieve all oura sleep entries
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve oura sleep entries with the following restrictions:
		/// -- Administrators can retrieve oura sleep entries for any participant
		/// -- Sub-administrators can retrieve oura sleep entries for participants of studies in their organization
		/// -- Supervisors can retrieve oura sleep entries for participants of their studies
		/// -- Therapists can retrieve oura sleep entries for participants they monitor
		/// -- Participants can only retrieve oura sleep entries of themselves
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		[HttpGet]
		[ProducesResponseType(typeof(OuraSleep), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllOuraSleeps([FromQuery(Name = "fromDate")] DateTime fromDate, [FromQuery(Name = "toDate")] DateTime toDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.ParticipantRole) {
				IUserDetails requestingUser = await _coadaptService.Participant.GetParticipantByUserIdAsync(userId);
				return fromDate == DateTime.MinValue ?
					Ok(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAsync(requestingUser.Id)) :
					Ok(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAndDateRangeAsync(requestingUser.Id, fromDate, toDate));
			}
			if (role == Role.AdministratorRole) {
				return fromDate == DateTime.MinValue ?
					Ok(await _coadaptService.OuraSleep.GetOuraSleepsAsync()) :
					Ok(await _coadaptService.OuraSleep.GetOuraSleepsByDateRangeAsync(fromDate, toDate));
			}
			var participants = await _coadaptService.Participant.GetAllParticipantsAsync();
			var filteredOuraSleeps = new List<OuraSleep>();
			foreach (var participant in participants) {
				if (role == Role.SubAdministratorRole) {
					if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredOuraSleeps.AddRange(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAsync(participant.Id));
					} else {
						filteredOuraSleeps.AddRange(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAndDateRangeAsync(participant.Id, fromDate, toDate));
					}
				} else if (role == Role.SupervisorRole) {
					if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredOuraSleeps.AddRange(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAsync(participant.Id));
					} else {
						filteredOuraSleeps.AddRange(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAndDateRangeAsync(participant.Id, fromDate, toDate));
					}
				} else if (role == Role.TherapistRole) {
					if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredOuraSleeps.AddRange(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAsync(participant.Id));
					} else {
						filteredOuraSleeps.AddRange(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAndDateRangeAsync(participant.Id, fromDate, toDate));
					}
				}
			}
			return Ok(filteredOuraSleeps);
		}

		/// <summary>
		/// Retrieve all oura sleep entries of a participant with given code
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve oura sleep entries of a given participant with the following restrictions:
		/// -- Administrators can retrieve oura sleep entries for any participant
		/// -- Sub-administrators can retrieve oura sleep entries for participants of studies in their organization
		/// -- Supervisors can retrieve oura sleep entries for participants of their studies
		/// -- Therapists can retrieve oura sleep entries for participants they monitor
		/// -- Participants can only retrieve oura sleep entries of themselves
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		[HttpGet("participant/{code}")]
		[ProducesResponseType(typeof(OuraSleep), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllOuraSleepsOfParticipant(string code, [FromQuery(Name = "fromDate")] DateTime fromDate, [FromQuery(Name = "toDate")] DateTime toDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A sub-administrator can retrieve only oura sleeps of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A supervisor can retrieve only oura sleeps of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(participant.Id, userId)) {
					return BadRequest("A participant can retrieve only own oura sleeps");
				}
			} else if (role == Role.TherapistRole) {
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only oura sleeps of monitored participants");
				}
			}
			return fromDate == DateTime.MinValue ?
				Ok(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAsync(participant.Id)) :
				Ok(await _coadaptService.OuraSleep.GetOuraSleepsByParticipantIdAndDateRangeAsync(participant.Id, fromDate, toDate));
		}

		/// <summary>
		///	Retrieve the oura sleep entry with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve an oura sleep entry by ID with the following restrictions:
		/// -- Administrators can retrieve an oura sleep entry for any participant
		/// -- Sub-administrators can retrieve an oura sleep entry for participants of studies in their organization
		/// -- Supervisors can retrieve an oura sleep entry for participants of their studies
		/// -- Therapists can retrieve oura sleep entries for participants they monitor
		/// -- Participants can only retrieve an oura sleep entry of themselves
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "OuraSleepById")]
		[ProducesResponseType(typeof(OuraSleep), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetOuraSleepById(int id) {
			var ouraSleep = await _coadaptService.OuraSleep.GetOuraSleepByIdAsync(id);
			if (ouraSleep.Id == 0) {
				_logger.LogWarn($"GetOuraSleepById: OuraSleep with ID {id} not found!");
				return NotFound("OuraSleep with requested ID does not exist");
			}
			ouraSleep.Participant = await _coadaptService.Participant.GetParticipantByIdAsync(ouraSleep.ParticipantId);
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(ouraSleep.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can view only oura sleeps of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(ouraSleep.ParticipantId, userId)) {
					return BadRequest("A supervisor can view only oura sleeps of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(ouraSleep.ParticipantId, userId)) {
					return BadRequest("A participant can view only own oura sleeps");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(ouraSleep.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only oura sleeps of monitored participants");
				}
			}
			return Ok(ouraSleep);
		}

		/// <summary>
		/// Create a new oura sleep entry
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can create a new oura sleep entry with the following restrictions:
		/// -- Administrators can create a new oura sleep entry for any participant
		/// -- Sub-administrators only create a new oura sleep entry for a participant to sites of their organization
		/// -- Supervisors only create a new oura sleep entry for a participant  to sites of their studies
		/// -- Therapists can create an oura sleep entry for participants they monitor
		/// -- Participants can only create a new oura sleep entry for themselves
		/// - Any number of the information fields can be omitted, to report just the new information available
		/// </remarks>
		/// <param name="ouraSleepRequest"></param>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateOuraSleep([FromBody] OuraSleepRequest ouraSleepRequest) {
			if (ouraSleepRequest == null) {
				_logger.LogError("CreateOuraSleep: OuraSleepRequest object sent from client is null.");
				return BadRequest("OuraSleepRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateOuraSleep: Invalid OuraSleepRequest object sent from client.");
				return BadRequest("Invalid OuraSleepRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(ouraSleepRequest.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can create only oura sleeps of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(ouraSleepRequest.ParticipantId, userId)) {
					return BadRequest("A supervisor can create only oura sleeps of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(ouraSleepRequest.ParticipantId, userId)) {
					return BadRequest("A participant can create only own oura sleeps");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(ouraSleepRequest.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can create only oura sleeps of monitored participants");
				}
			}
			var ouraSleep = new OuraSleep();
			ouraSleep.FromRequest(ouraSleepRequest);
			_coadaptService.OuraSleep.CreateOuraSleep(ouraSleep);
			await _coadaptService.SaveAsync();
			return CreatedAtRoute("OuraSleepById", new { id = ouraSleep.Id }, ouraSleep);
		}

		/// <summary>
		/// Delete the oura sleep entry with given id.
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can delete an oura sleep entry with the following restrictions:
		/// -- Administrators can delete an oura sleep entry of any participant
		/// -- Sub-administrators can delete an oura sleep entry of participants of studies in their organization
		/// -- Supervisors can delete an oura sleep entry of participants of their studies
		/// -- Therapists can delete an oura sleep entry of participants they monitor
		/// -- Participants can only delete an oura sleep entry of themselves
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteOuraSleep(int id) {
			var ouraSleep = await _coadaptService.OuraSleep.GetOuraSleepByIdAsync(id);
			if (ouraSleep.Id == 0) {
				_logger.LogDebug($"DeleteOuraSleep: OuraSleep with id {id} not found.");
				return NotFound("OuraSleep with requested id does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(ouraSleep.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can delete only oura sleeps of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(ouraSleep.ParticipantId, userId)) {
					return BadRequest("A supervisor can delete only oura sleeps of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(ouraSleep.ParticipantId, userId)) {
					return BadRequest("A participant can delete only own oura sleeps");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(ouraSleep.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can delete only oura sleeps of monitored participants");
				}
			}
			_coadaptService.OuraSleep.DeleteOuraSleep(ouraSleep);
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
