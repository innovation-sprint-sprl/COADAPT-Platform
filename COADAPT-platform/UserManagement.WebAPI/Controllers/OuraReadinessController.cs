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
	[Route("1.0/ouraReadiness")]
	[ApiController]
	[Authorize(Policy = "Everyone")]
	public class OuraReadinessController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;

		public OuraReadinessController(ILoggerManager logger, IRepositoryWrapper repository) {
			_logger = logger;
			_coadaptService = repository;
		}

		/// <summary>
		/// Retrieve all oura readiness entries
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve oura readiness entries with the following restrictions:
		/// -- Administrators can retrieve oura readiness entries for any participant
		/// -- Sub-administrators can retrieve oura readiness entries for participants of studies in their organization
		/// -- Supervisors can retrieve oura readiness entries for participants of their studies
		/// -- Therapists can retrieve oura readiness entries for participants they monitor
		/// -- Participants can only retrieve oura readiness entries of themselves
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="fromDate"></param>
		[HttpGet]
		[ProducesResponseType(typeof(OuraReadiness), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllOuraReadinesses([FromQuery(Name = "fromDate")] DateTime fromDate, [FromQuery(Name = "toDate")] DateTime toDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.ParticipantRole) {
				IUserDetails requestingUser = await _coadaptService.Participant.GetParticipantByUserIdAsync(userId);
				return fromDate == DateTime.MinValue ?
					Ok(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAsync(requestingUser.Id)) :
					Ok(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAndDateRangeAsync(requestingUser.Id, fromDate, toDate));
			}
			if (role == Role.AdministratorRole) {
				return fromDate == DateTime.MinValue ?
					Ok(await _coadaptService.OuraReadiness.GetOuraReadinessesAsync()) :
					Ok(await _coadaptService.OuraReadiness.GetOuraReadinessesByDateRangeAsync(fromDate, toDate));
			}
			var participants = await _coadaptService.Participant.GetAllParticipantsAsync();
			var filteredOuraReadinesses = new List<OuraReadiness>();
			foreach (var participant in participants) {
				if (role == Role.SubAdministratorRole) {
					if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredOuraReadinesses.AddRange(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAsync(participant.Id));
					} else {
						filteredOuraReadinesses.AddRange(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAndDateRangeAsync(participant.Id, fromDate, toDate));
					}
				} else if (role == Role.SupervisorRole) {
					if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredOuraReadinesses.AddRange(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAsync(participant.Id));
					} else {
						filteredOuraReadinesses.AddRange(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAndDateRangeAsync(participant.Id, fromDate, toDate));
					}
				} else if (role == Role.TherapistRole) {
					if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredOuraReadinesses.AddRange(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAsync(participant.Id));
					} else {
						filteredOuraReadinesses.AddRange(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAndDateRangeAsync(participant.Id, fromDate, toDate));
					}
				}
			}
			return Ok(filteredOuraReadinesses);
		}

		/// <summary>
		/// Retrieve all oura readiness entries of a participant with given code
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve oura readiness entries of a given participant with the following restrictions:
		/// -- Administrators can retrieve oura readiness entries for any participant
		/// -- Sub-administrators can retrieve oura readiness entries for participants of studies in their organization
		/// -- Supervisors can retrieve oura readiness entries for participants of their studies
		/// -- Therapists can retrieve oura readiness entries for participants they monitor
		/// -- Participants can only retrieve oura readiness entries of themselves
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="fromDate"></param>
		[HttpGet("participant/{code}")]
		[ProducesResponseType(typeof(OuraReadiness), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllOuraReadinessesOfParticipant(string code, [FromQuery(Name = "fromDate")] DateTime fromDate, [FromQuery(Name = "toDate")] DateTime toDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A sub-administrator can retrieve only oura readinesses of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A supervisor can retrieve only oura readinesses of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(participant.Id, userId)) {
					return BadRequest("A participant can retrieve only own oura readinesses");
				}
			} else if (role == Role.TherapistRole) {
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only oura readinesses of monitored participants");
				}
			}
			return fromDate == DateTime.MinValue ?
				Ok(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAsync(participant.Id)) :
				Ok(await _coadaptService.OuraReadiness.GetOuraReadinessesByParticipantIdAndDateRangeAsync(participant.Id, fromDate, toDate));
		}

		/// <summary>
		///	Retrieve the oura readiness entry with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve an oura readiness entry by ID with the following restrictions:
		/// -- Administrators can retrieve an oura readiness entry for any participant
		/// -- Sub-administrators can retrieve an oura readiness entry for participants of studies in their organization
		/// -- Supervisors can retrieve an oura readiness entry for participants of their studies
		/// -- Therapists can retrieve oura readiness entries for participants they monitor
		/// -- Participants can only retrieve an oura readiness entry of themselves
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "OuraReadinessById")]
		[ProducesResponseType(typeof(OuraReadiness), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetOuraReadinessById(int id) {
			var ouraReadiness = await _coadaptService.OuraReadiness.GetOuraReadinessByIdAsync(id);
			if (ouraReadiness.Id == 0) {
				_logger.LogWarn($"GetOuraReadinessById: OuraReadiness with ID {id} not found!");
				return NotFound("OuraReadiness with requested ID does not exist");
			}
			ouraReadiness.Participant = await _coadaptService.Participant.GetParticipantByIdAsync(ouraReadiness.ParticipantId);
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(ouraReadiness.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can view only oura readinesses of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(ouraReadiness.ParticipantId, userId)) {
					return BadRequest("A supervisor can view only oura readinesses of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(ouraReadiness.ParticipantId, userId)) {
					return BadRequest("A participant can view only own oura readinesses");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(ouraReadiness.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only oura readinesses of monitored participants");
				}
			}
			return Ok(ouraReadiness);
		}

		/// <summary>
		/// Create a new oura readiness entry
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can create a new oura readiness entry with the following restrictions:
		/// -- Administrators can create a new oura readiness entry for any participant
		/// -- Sub-administrators only create a new oura readiness entry for a participant to sites of their organization
		/// -- Supervisors only create a new oura readiness entry for a participant  to sites of their studies
		/// -- Therapists can create a new oura readiness entry for participants they monitor
		/// -- Participants can only create a new oura readiness entry for themselves
		/// - Any number of the information fields can be omitted, to report just the new information available
		/// </remarks>
		/// <param name="ouraReadinessRequest"></param>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateOuraReadiness([FromBody] OuraReadinessRequest ouraReadinessRequest) {
			if (ouraReadinessRequest == null) {
				_logger.LogError("CreateOuraReadiness: OuraReadinessRequest object sent from client is null.");
				return BadRequest("OuraReadinessRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateOuraReadiness: Invalid OuraReadinessRequest object sent from client.");
				return BadRequest("Invalid OuraReadinessRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(ouraReadinessRequest.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can create only oura readinesses of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(ouraReadinessRequest.ParticipantId, userId)) {
					return BadRequest("A supervisor can create only oura readinesses of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(ouraReadinessRequest.ParticipantId, userId)) {
					return BadRequest("A participant can create only own oura readinesses");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(ouraReadinessRequest.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can create only oura readinesses of monitored participants");
				}
			}

			var ouraReadiness = new OuraReadiness();

			if (!_coadaptService.OuraReadiness.Exists(ouraReadinessRequest.ParticipantId, ouraReadinessRequest.SummaryDate)) {
				ouraReadiness.FromRequest(ouraReadinessRequest);
				_coadaptService.OuraReadiness.CreateOuraReadiness(ouraReadiness);
				await _coadaptService.SaveAsync();
			}
			
			return CreatedAtRoute("OuraReadinessById", new { id = ouraReadiness.Id }, ouraReadiness);
		}

		/// <summary>
		/// Delete the oura readiness entry with given id.
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can delete an oura readiness entry with the following restrictions:
		/// -- Administrators can delete an oura readiness entry of any participant
		/// -- Sub-administrators can delete an oura readiness entry of participants of studies in their organization
		/// -- Supervisors can delete an oura readiness entry of participants of their studies
		/// -- Therapists can delete an oura readiness entry of participants they monitor
		/// -- Participants can only delete an oura readiness entry of themselves
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteOuraReadiness(int id) {
			var ouraReadiness = await _coadaptService.OuraReadiness.GetOuraReadinessByIdAsync(id);
			if (ouraReadiness.Id == 0) {
				_logger.LogDebug($"DeleteOuraReadiness: OuraReadiness with id {id} not found.");
				return NotFound("OuraReadiness with requested id does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(ouraReadiness.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can delete only oura readinesses of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(ouraReadiness.ParticipantId, userId)) {
					return BadRequest("A supervisor can delete only oura readinesses of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(ouraReadiness.ParticipantId, userId)) {
					return BadRequest("A participant can delete only own oura readinesses");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(ouraReadiness.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can delete only oura readinesses of monitored participants");
				}
			}
			_coadaptService.OuraReadiness.DeleteOuraReadiness(ouraReadiness);
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
