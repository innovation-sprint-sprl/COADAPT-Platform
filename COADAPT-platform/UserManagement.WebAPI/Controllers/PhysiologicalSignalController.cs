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
	[Route("1.0/physiologicalSignals")]
	[ApiController]
	[Authorize(Policy = "Everyone")]
	public class PhysiologicalSignalController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;

		public PhysiologicalSignalController(ILoggerManager logger, IRepositoryWrapper repository) {
			_logger = logger;
			_coadaptService = repository;
		}

		/// <summary>
		/// Retrieve all physiological signal entries
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve physiological signal entries with the following restrictions:
		/// -- Administrators can retrieve physiological signal entries for any participant
		/// -- Sub-administrators can retrieve physiological signal entries for participants of studies in their organization
		/// -- Supervisors can retrieve physiological signal entries for participants of their studies
		/// -- Therapists can retrieve physiological signal entries for participants they monitor
		/// -- Participants can only retrieve physiological signal entries of themselves
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="fromDate"></param>
		[HttpGet]
		[ProducesResponseType(typeof(PhysiologicalSignal), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllPhysiologicalSignals([FromQuery(Name = "fromDate")] DateTime fromDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.ParticipantRole) {
				IUserDetails requestingUser = await _coadaptService.Participant.GetParticipantByUserIdAsync(userId);
				return fromDate == DateTime.MinValue ?
					Ok(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsByParticipantIdAsync(requestingUser.Id)) :
					Ok(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsAfterDateByParticipantIdAsync(fromDate, requestingUser.Id));
			}
			if (role == Role.AdministratorRole) {
				return fromDate == DateTime.MinValue ? 
					Ok(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsAsync()) : 
					Ok(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsAfterDateAsync(fromDate));
			}
			var participants = await _coadaptService.Participant.GetAllParticipantsAsync();
			var filteredPhysiologicalSignals = new List<PhysiologicalSignal>();
			foreach (var participant in participants) {
				if (role == Role.SubAdministratorRole) {
					if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredPhysiologicalSignals.AddRange(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsByParticipantIdAsync(participant.Id));
					} else {
						filteredPhysiologicalSignals.AddRange(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsAfterDateByParticipantIdAsync(fromDate, participant.Id));
					}
				} else if (role == Role.SupervisorRole) {
					if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredPhysiologicalSignals.AddRange(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsByParticipantIdAsync(participant.Id));
					} else {
						filteredPhysiologicalSignals.AddRange(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsAfterDateByParticipantIdAsync(fromDate, participant.Id));
					}
				} else if (role == Role.TherapistRole) {
					if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
						continue;
					}
					if (fromDate == DateTime.MinValue) {
						filteredPhysiologicalSignals.AddRange(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsByParticipantIdAsync(participant.Id));
					} else {
						filteredPhysiologicalSignals.AddRange(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsAfterDateByParticipantIdAsync(fromDate, participant.Id));
					}
				}
			}
			return Ok(filteredPhysiologicalSignals);
		}

		/// <summary>
		/// Retrieve all physiological signal entries of a participant with given code
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve physiological signal entries of a given participant with the following restrictions:
		/// -- Administrators can retrieve physiological signal entries for any participant
		/// -- Sub-administrators can retrieve physiological signal entries for participants of studies in their organization
		/// -- Supervisors can retrieve physiological signal entries for participants of their studies
		/// -- Therapists can retrieve physiological signal entries for participants they monitor
		/// -- Participants can only retrieve physiological signal entries of themselves
		/// - An optional fromDate query parameter can be used to retrieve entries from that date onwards
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="fromDate"></param>
		[HttpGet("participant/{code}")]
		[ProducesResponseType(typeof(PhysiologicalSignal), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllPhysiologicalSignalsOfParticipant(string code, [FromQuery(Name = "fromDate")] DateTime fromDate) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A sub-administrator can retrieve only physiological signals of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(participant.Id, userId)) {
					return BadRequest("A supervisor can retrieve only physiological signals of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(participant.Id, userId)) {
					return BadRequest("A participant can retrieve only own physiological signals");
				}
			} else if (role == Role.TherapistRole) {
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only physiological signals of monitored participants");
				}
			}
			return fromDate == DateTime.MinValue ?
				Ok(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsByParticipantIdAsync(participant.Id)) :
				Ok(await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalsAfterDateByParticipantIdAsync(fromDate, participant.Id));
		}

		/// <summary>
		///	Retrieve the physiological signal entry with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can retrieve a physiological signal entry by ID with the following restrictions:
		/// -- Administrators can retrieve a physiological signal entry for any participant
		/// -- Sub-administrators can retrieve a physiological signal entry for participants of studies in their organization
		/// -- Supervisors can retrieve a physiological signal entry for participants of their studies
		/// -- Therapists can retrieve physiological signal entries for participants they monitor
		/// -- Participants can only retrieve a physiological signal entry of themselves
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "PhysiologicalSignalById")]
		[ProducesResponseType(typeof(PhysiologicalSignal), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetPhysiologicalSignalById(int id) {
			var physiologicalSignal = await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalByIdAsync(id);
			if (physiologicalSignal.Id == 0) {
				_logger.LogWarn($"GetPhysiologicalSignalById: PhysiologicalSignal with ID {id} not found!");
				return NotFound("PhysiologicalSignal with requested ID does not exist");
			}
			physiologicalSignal.Participant = await _coadaptService.Participant.GetParticipantByIdAsync(physiologicalSignal.ParticipantId);
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(physiologicalSignal.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can view only physiological signals of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(physiologicalSignal.ParticipantId, userId)) {
					return BadRequest("A supervisor can view only physiological signals of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(physiologicalSignal.ParticipantId, userId)) {
					return BadRequest("A participant can view only own physiological signals");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(physiologicalSignal.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can retrieve only physiological signals of monitored participants");
				}
			}
			return Ok(physiologicalSignal);
		}

		/// <summary>
		/// Create a new physiological signal entry
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can create a new physiological signal entry with the following restrictions:
		/// -- Administrators can create a new physiological signal entry for any participant
		/// -- Sub-administrators only create a new physiological signal entry for a participant to sites of their organization
		/// -- Supervisors only create a new physiological signal entry for a participant  to sites of their studies
		/// -- Therapists can create a physiological signal entry for participants they monitor
		/// -- Participants can only create a new physiological signal entry for themselves
		/// - Any number of the information fields can be omitted, to report just the new information available
		/// </remarks>
		/// <param name="physiologicalSignalRequest"></param>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreatePhysiologicalSignal([FromBody]PhysiologicalSignalRequest physiologicalSignalRequest) {
			if (physiologicalSignalRequest == null) {
				_logger.LogError("CreatePhysiologicalSignal: PhysiologicalSignalRequest object sent from client is null.");
				return BadRequest("PhysiologicalSignalRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreatePhysiologicalSignal: Invalid PhysiologicalSignalRequest object sent from client.");
				return BadRequest("Invalid PhysiologicalSignalRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(physiologicalSignalRequest.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can create only psychological signals of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(physiologicalSignalRequest.ParticipantId, userId)) {
					return BadRequest("A supervisor can create only psychological signals of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(physiologicalSignalRequest.ParticipantId, userId)) {
					return BadRequest("A participant can create only own psychological signals");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(physiologicalSignalRequest.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can create only psychological signals of monitored participants");
				}
			}
			var physiologicalSignal = new PhysiologicalSignal();
			physiologicalSignal.FromRequest(physiologicalSignalRequest);
			_coadaptService.PhysiologicalSignal.CreatePhysiologicalSignal(physiologicalSignal);
			await _coadaptService.SaveAsync();
			return CreatedAtRoute("PhysiologicalSignalById", new { id = physiologicalSignal.Id }, physiologicalSignal);
		}

		/// <summary>
		/// Delete the physiological signal entry with given id.
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - All users can delete a new physiological signal entry with the following restrictions:
		/// -- Administrators can delete a physiological signal entry of any participant
		/// -- Sub-administrators can delete a physiological signal entry of participants of studies in their organization
		/// -- Supervisors can delete a physiological signal entry of participants of their studies
		/// -- Therapists can delete a physiological signal entry of participants they monitor
		/// -- Participants can only delete a physiological signal entry of themselves
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeletePhysiologicalSignal(int id) {
			var physiologicalSignal = await _coadaptService.PhysiologicalSignal.GetPhysiologicalSignalByIdAsync(id);
			if (physiologicalSignal.Id == 0) {
				_logger.LogDebug($"DeletePhysiologicalSignal: PhysiologicalSignal with id {id} not found.");
				return NotFound("PhysiologicalSignal with requested id does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				if (!await ParticipantInOrganizationOfUserIdAsync(physiologicalSignal.ParticipantId, userId)) {
					return BadRequest("A sub-administrator can delete only physiological signals of a participant of own organization");
				}
			} else if (role == Role.SupervisorRole) {
				if (!await ParticipantInStudiesOfUserIdAsync(physiologicalSignal.ParticipantId, userId)) {
					return BadRequest("A supervisor can delete only physiological signals of a participant of own studies");
				}
			} else if (role == Role.ParticipantRole) {
				if (!await ParticipantSameAsUserIdAsync(physiologicalSignal.ParticipantId, userId)) {
					return BadRequest("A participant can delete only own physiological signals");
				}
			} else if (role == Role.TherapistRole) {
				var participant = await _coadaptService.Participant.GetParticipantByIdAsync(physiologicalSignal.ParticipantId);
				if (!await ParticipantMonitoredByTherapistOfUserIdAsync(participant, userId)) {
					return BadRequest("A therapist can delete only physiological signals of monitored participants");
				}
			}
			_coadaptService.PhysiologicalSignal.DeletePhysiologicalSignal(physiologicalSignal);
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
