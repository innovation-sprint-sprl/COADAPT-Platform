using Constants;
using Contracts.Logger;
using Contracts.Repository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiModels;
using System;

namespace UserManagement.WebAPI.Controllers {

	[Route("1.0/participants")]
	[ApiController]
	[Authorize(Policy = "Everyone")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
	public class ParticipantController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public ParticipantController(ILoggerManager logger, IRepositoryWrapper repository,
			UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
			_logger = logger;
			_coadaptService = repository;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		/// <summary>
		/// Retrieve participants
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators, supervisors or therapists can retrieve all participants
		/// -- Administrators can retrieve all participants
		/// -- Sub-administrators only retrieve participants to sites of their organization
		/// -- Supervisors only retrieve participants to sites of their studies
		/// -- Therapists only retrieve participants assigned to them
		/// </remarks>
		[Authorize(Policy = "Therapist")]
		[HttpGet]
		[ProducesResponseType(typeof(ParticipantListResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllParticipants() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			_logger.LogInfo(role);
			if (role == Role.AdministratorRole) {
				return Ok(await _coadaptService.Participant.GetParticipantListAsync());
			}
			if (role == Role.SubAdministratorRole || role == Role.SupervisorRole) {
				List<Study> studies = new List<Study>();
				if (role == Role.SubAdministratorRole) {
					var requestingSubAdmin = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
					var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(requestingSubAdmin.Id);
					studies.AddRange(await _coadaptService.Study.GetStudiesByOrganizationIdAsync(organization.Id));
				} else {
					var requestingSupervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
					studies.AddRange(await _coadaptService.Study.GetStudiesBySupervisorIdAsync(requestingSupervisor.Id));
				}
				List<StudyParticipant> studyParticipants = new List<StudyParticipant>();
				foreach (var study in studies) {
					studyParticipants.AddRange(await _coadaptService.StudyParticipant.StudyParticipantsByStudy(study.Id, false));
				}
				IList<ParticipantListResponse> participants = new List<ParticipantListResponse>();
				foreach (var studyParticipant in studyParticipants) {
					var participant = await _coadaptService.Participant.GetParticipantListItemByIdAsync(studyParticipant.ParticipantId);
					if (!participants.Contains(participant)) {
						participants.Add(participant);
					}
				}
				return Ok(participants);
			}
			// Requesting user is a therapist
			var requestingTherapist = await _coadaptService.Therapist.GetTherapistByUserIdAsync(userId);
			return Ok(await _coadaptService.Participant.GetParticipantsByTherapistIdAsync(requestingTherapist.Id));
		}

		/// <summary>
		///	Retrieve the participant with given code
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can retrieve a participant by code
		/// </remarks>
		/// <param name="code"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpGet("{code}" , Name = "ParticipantByCode")]
		[ProducesResponseType(typeof(Participant), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetParticipantByCode(string code) {
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (participant.IsEmptyObject()) {
				_logger.LogWarn($"GetParticipantByCode: Participant with code {code} not found!");
				return NotFound("Participant with requested code does not exist");
			}
			participant.User = await _userManager.FindByIdAsync(participant.UserId);
			if (participant.TherapistId.HasValue) {
				participant.Therapist = await _coadaptService.Therapist.GetTherapistByIdAsync(participant.TherapistId.Value);
			}
			participant.StudyParticipants = (ICollection<StudyParticipant>)await _coadaptService.StudyParticipant.StudyParticipantsByParticipant(participant.Id);
			return Ok(participant);
		}

		/// <summary>
		/// Create a new participant
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can create a new participant
		/// -- Administrators can add participant to any site
		/// -- Sub-administrators only add participant to sites of their organization
		/// -- Supervisors only add participants to sites of their studies
		/// - Username must be unique
		/// - A valid participant code must be provided (currently just non-empty)
		/// - Ignore the therapist ID if a therapist is to be assigned at a later time
		/// - A participant can be added to any number of sites
		/// -- Sites must belong to different studies; sites belonging to the same study are ignored
		/// -- Sites where the participant already belongs are ignored
		/// </remarks>
		/// <param name="participantCreateRequest"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateParticipant([FromBody]ParticipantCreateRequest participantCreateRequest) {
			if (participantCreateRequest == null) {
				_logger.LogError("CreateParticipant: ParticipantCreateRequest object sent from client is null.");
				return BadRequest("ParticipantRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateParticipant: Invalid ParticipantCreateRequest object sent from client.");
				return BadRequest("Invalid ParticipantCreateRequest object");
			}

			#region participant checks
			if (string.IsNullOrEmpty(participantCreateRequest.Code)) {
				_logger.LogError("CreateParticipant: Invalid participant code sent from client.");
				return BadRequest("Invalid participant code");
			}
			Participant participant = await _coadaptService.Participant.GetParticipantByCodeAsync(participantCreateRequest.Code);
			if (!participant.IsEmptyObject()) {
				_logger.LogError("CreateParticipant: Participant code sent from client already in use.");
				return BadRequest("Provided participant code already in use");
			}
			var therapist = new Therapist();
			if (participantCreateRequest.TherapistId != 0 && participantCreateRequest.TherapistId != null) {
				therapist = await _coadaptService.Therapist.GetTherapistByIdAsync((int)participantCreateRequest.TherapistId);
			}
			var user = await _userManager.FindByNameAsync(participantCreateRequest.UserName);
			if (user != null) {
				_logger.LogError("CreateParticipant: username already exists.");
				return BadRequest("Username already exists");
			}
			user = new IdentityUser { UserName = participantCreateRequest.UserName };
			var passwordValidator = new PasswordValidator<IdentityUser>();
			if (!(await passwordValidator.ValidateAsync(_userManager, null, participantCreateRequest.Password)).Succeeded) {
				_logger.LogError("CreateParticipant: Provided password is not strong enough.");
				return BadRequest("Provided password is not strong enough");
			}
			#endregion

			#region studyparticipant checks
			if (participantCreateRequest.StartDate > participantCreateRequest.EndDate) {
				_logger.LogError("CreateParticipant: The participation start date must be before the participation end date");
				return BadRequest("The participation start date must be before the participation end date");
			}

			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			IUserDetails requestingUser;
			if (role == Role.AdministratorRole) {
				requestingUser = await _coadaptService.Administrator.GetAdministratorByUserIdAsync(userId);
			} else if (role == Role.SubAdministratorRole) {
				requestingUser = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			} else {
				requestingUser = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			}

			var site = await _coadaptService.Site.GetSiteByIdAsync(participantCreateRequest.SiteId);
			if (site.IsEmptyObject()) {
				_logger.LogError("CreateParticipant: The requested site does not exist.");
				return BadRequest("A requested site does not exist");
			}
			var group = await _coadaptService.Group.GetGroupByIdAsync(participantCreateRequest.GroupId);
			if (group.IsEmptyObject()) {
				_logger.LogError("CreateParticipant: The requested group does not exist.");
				return BadRequest("A requested group does not exist");
			}
			if (site.StudyId != group.StudyId) {
				_logger.LogError("CreateParticipant: The requested site/group pair belongs to different studies.");
				return BadRequest("The requested site/group pair belongs to different studies");
			}
			var study = await _coadaptService.Study.GetStudyByIdAsync(site.StudyId);
			var organization = await _coadaptService.Organization.GetOrganizationByIdAsync(study.OrganizationId);
			if (role == Role.SubAdministratorRole && organization.SubAdministratorId != requestingUser.Id) {
				_logger.LogError("CreateParticipant: The requested site/group pair belongs to study outside the sub-administrator's organization.");
				return BadRequest("The requested site/group pair belongs to study outside the sub-administrator's organization");
			}
			if (role == Role.SupervisorRole && study.SupervisorId != requestingUser.Id) {
				_logger.LogError("CreateParticipant: The requested site/group pair belongs to study supervised by a different supervisor.");
				return BadRequest("The requested site/group pair belongs to study supervised by a different supervisor");
			}
			#endregion

			#region creation of user/participant
			await _userManager.CreateAsync(user, participantCreateRequest.Password);

			int? therapistId = null;
			if (therapist.IsEmptyObject() || participantCreateRequest.TherapistId == 0) {
				therapistId = null;
			} else {
				therapistId = therapist.Id;
			}
			participant = new Participant {
				UserId = user.Id,
				Code = participantCreateRequest.Code,
				TherapistId = therapistId,
				Gender = participantCreateRequest.Gender,
				DateOfBirth = participantCreateRequest.DateOfBirth ?? participant.DateOfBirth,
				DateOfFirstJob = participantCreateRequest.DateOfFirstJob ?? participant.DateOfFirstJob
			};

			_coadaptService.Participant.CreateParticipant(participant);
			if (!await _roleManager.RoleExistsAsync(Role.ParticipantRole)) {
				await _roleManager.CreateAsync(new IdentityRole(Role.ParticipantRole));
			}
			await _userManager.AddToRoleAsync(user, Role.ParticipantRole);
			#endregion

			#region creation of studyparticipant
			_coadaptService.StudyParticipant.CreateStudyParticipant(new StudyParticipant() {
				StudyId = study.Id,
				SiteId = site.Id,
				GroupId = group.Id,
				ParticipantId = participant.Id,
				StartDate = participantCreateRequest.StartDate ?? new DateTime(2000, 1, 1),
				EndDate = participantCreateRequest.EndDate ?? new DateTime(2000, 1, 1),
				DateOfCurrentJob = participantCreateRequest.DateOfCurrentJob ?? new DateTime(2000, 1, 1),
				Education = participantCreateRequest.Education,
				JobType = participantCreateRequest.JobType,
				MaritalStatus = participantCreateRequest.MaritalStatus,
				RegionOfResidence = participantCreateRequest.Region
			});
			#endregion

			await _coadaptService.SaveAsync();
			if (therapist.IsEmptyObject() || participantCreateRequest.TherapistId == 0) {
				participant.Therapist = therapist;
			}
			participant.StudyParticipants = (ICollection<StudyParticipant>)await _coadaptService.StudyParticipant.StudyParticipantsByParticipant(participant.Id);
			return CreatedAtRoute("ParticipantByCode", new { code = participant.Code }, participant);
		}

		/// <summary>
		/// Delete the participant with given code.
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can delete a participant
		/// </remarks>
		/// <param name="code"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpDelete("{code}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteParticipantByCode(string code) {
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (participant.IsEmptyObject()) {
				_logger.LogDebug($"DeleteParticipant: Participant with code {code} not found.");
				return NotFound("Participant with requested code does not exist");
			}
			var user = await _userManager.FindByIdAsync(participant.UserId);
			var logins = await _userManager.GetLoginsAsync(user);
			var rolesForUser = await _userManager.GetRolesAsync(user);
			IdentityResult result;
			foreach (var login in logins) {
				result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
				if (result != IdentityResult.Success) {
					return StatusCode(StatusCodes.Status500InternalServerError, "Cannot delete user logins");
				}
			}
			foreach (var item in rolesForUser) {
				result = await _userManager.RemoveFromRoleAsync(user, item);
				if (result != IdentityResult.Success) {
					return StatusCode(StatusCodes.Status500InternalServerError, "Cannot delete user roles");
				}
			}
			foreach (StudyParticipant studyParticipant in await _coadaptService.StudyParticipant.StudyParticipantsByParticipant(participant.Id)) {
				_coadaptService.StudyParticipant.DeleteStudyParticipant(studyParticipant);
			}
			foreach (PsychologicalReport psychologicalReport in await _coadaptService.PsychologicalReport.GetPsychologicalReportsByParticipantIdAsync(participant.Id)) {
				_coadaptService.PsychologicalReport.DeletePsychologicalReport(psychologicalReport);
			}
			_coadaptService.Participant.DeleteParticipant(participant);
			result = await _userManager.DeleteAsync(user);
			if (result != IdentityResult.Success) {
				return StatusCode(StatusCodes.Status500InternalServerError, "Cannot delete user");
			}
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Assign a participant with the given code to a site/group pair with the given IDs
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can assign a participant to a site/group pair
		/// -- If requesting user is an administrator, the participant may be assigned to the requested site/group pair
		/// -- If requesting user is a sub-administrator, the participant may be assigned only the requested site/group pair of studies belonging to own organization
		/// -- If requesting user is a supervisor, the participant may be assigned only the requested site/group pair belonging to own studies
		/// - Participant must not already be an active member of a site of the study in which the requested site/group pair belongs
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="siteId"></param>
		/// <param name="groupId"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpPut("{code}/assignToSiteGroup/{siteId}/{groupId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> AssignToSiteGroup(string code, int siteId, int groupId) {
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (participant.IsEmptyObject()) {
				_logger.LogError("AssignToSiteGroup: Participant with requested ID does not exist.");
				return NotFound($"Participant {code} does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			IUserDetails requestingUser;
			if (role == Role.AdministratorRole) {
				requestingUser = await _coadaptService.Administrator.GetAdministratorByUserIdAsync(userId);
			} else if (role == Role.SubAdministratorRole) {
				requestingUser = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			} else {
				requestingUser = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			}
			var site = await _coadaptService.Site.GetSiteByIdAsync(siteId);
			if (site.IsEmptyObject()) {
				_logger.LogError($"AssignToSiteGroup: Site {siteId} does not exist!");
				return BadRequest($"Site {siteId} does not exist");
			}
			var group = await _coadaptService.Group.GetGroupByIdAsync(groupId);
			if (group.IsEmptyObject()) {
				_logger.LogError($"AssignToSiteGroup: Group {groupId} does not exist!");
				return BadRequest($"Site {groupId} does not exist");
			}
			if (group.StudyId != site.StudyId) {
				_logger.LogError("AssignToSiteGroup: Site/group pair does not belong to the same study!");
				return BadRequest("Site/group pair does not belong to the same study");
			}
			var study = await _coadaptService.Study.GetStudyByIdAsync(site.StudyId);
			var organization = await _coadaptService.Organization.GetOrganizationByIdAsync(study.OrganizationId);
			if (role == Role.SubAdministratorRole && organization.SubAdministratorId != requestingUser.Id) {
				_logger.LogWarn($"AssignToSiteGroup: Currently logged in sub-administrator is not assigned to an organization hosting the requested site/group pair!");
				return BadRequest("Currently logged in sub-administrator is not assigned to an organization hosting the requested site/group pair");
			}
			if (role == Role.SupervisorRole && study.SupervisorId != requestingUser.Id) {
				_logger.LogWarn($"AssignToSiteGroup: Currently logged in supervisor is not assigned to a study running at the requested site/group pair!");
				return BadRequest("Currently logged in supervisor is not assigned to a study running at the requested site/group pair");
			}
			if ((await _coadaptService.StudyParticipant.StudyParticipantsByParticipant(participant.Id))
				.Any(sp => sp.StudyId == study.Id && sp.EndDate == null)) {
				_logger.LogWarn($"AssignToSiteGroup: Participant {participant.Id} already an active member of the study the site/group pair belongs to!");
				return BadRequest($"AssignToSiteGroup: Participant {participant.Id} already an active member of the study the site/group pair belongs to");
			}
			_coadaptService.StudyParticipant.CreateStudyParticipant(new StudyParticipant { 
				StudyId = study.Id,
				ParticipantId = participant.Id,
				SiteId = site.Id,
				GroupId = group.Id,
				StartDate = DateTime.Now,
				EndDate = null });
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// End the involvement of a participant with the given code with a site/group pair with the given IDs
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can end the involvement of a participant with a site/group pair
		/// -- If requesting user is an administrator, the participant's involvement with the requested site/group pair is ended
		/// -- If requesting user is a sub-administrator, the participant's involvement is ended only if the requested site/group pair participates in studies belonging to own organization
		/// -- If requesting user is a supervisor, the participant's involvement is ended only if the requested site/group pair belongs to own studies
		/// - Participant must be an active member of the site/group pair of which the involvement is ended
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="siteId"></param>
		/// <param name="groupId"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpPut("{code}/endFromSiteGroup/{siteId}/{groupId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> EndFromSite(string code, int siteId, int groupId) {
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (participant.IsEmptyObject()) {
				_logger.LogError("EndFromSiteGroup: Participant with requested ID does not exist.");
				return NotFound("Participant with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			IUserDetails requestingUser;
			if (role == Role.AdministratorRole) {
				requestingUser = await _coadaptService.Administrator.GetAdministratorByUserIdAsync(userId);
			} else if (role == Role.SubAdministratorRole) {
				requestingUser = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			} else {
				requestingUser = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			}
			var site = await _coadaptService.Site.GetSiteByIdAsync(siteId);
			if (site.IsEmptyObject()) {
				_logger.LogError($"EndFromSiteGroup: Site {siteId} does not exist!");
				return BadRequest($"Site {siteId} does not exist");
			}
			var group = await _coadaptService.Group.GetGroupByIdAsync(groupId);
			if (group.IsEmptyObject()) {
				_logger.LogError($"EndFromSiteGroup: Group {groupId} does not exist!");
				return BadRequest($"Group {groupId} does not exist");
			}
			if (group.StudyId != site.StudyId) {
				_logger.LogError("EndFromSiteGroup: Site/group pair does not belong to the same study!");
				return BadRequest("Site/group pair does not belong to the same study");
			}
			var study = await _coadaptService.Study.GetStudyByIdAsync(site.StudyId);
			var organization = await _coadaptService.Organization.GetOrganizationByIdAsync(study.OrganizationId);
			if (role == Role.SubAdministratorRole && organization.SubAdministratorId != requestingUser.Id) {
				_logger.LogWarn("EndFromSiteGroup: Currently logged in sub-administrator is not assigned to an organization hosting the requested site/group pair!");
				return BadRequest("Currently logged in sub-administrator is not assigned to an organization hosting the requested site/group pair");
			}
			if (role == Role.SupervisorRole && study.SupervisorId != requestingUser.Id) {
				_logger.LogWarn("EndFromSite: Currently logged in supervisor is not assigned to a study running at the requested site/group pair!");
				return BadRequest("Currently logged in supervisor is not assigned to a study running at the requested site/group pair");
			}
			var studyParticipant = await
				_coadaptService.StudyParticipant.StudyParticipantByParticipantAndStudy(participant.Id, study.Id);
			if (studyParticipant.StudyId == 0) {
				_logger.LogError($"EndFromSiteGroup: Participant {code} is not a member of the requested site/group pair!");
				return BadRequest($"EndFromSiteGroup: Participant {code} is not a member of the requested site/group pair");
			}
			studyParticipant.Abandoned = true;
			_coadaptService.StudyParticipant.Update(studyParticipant);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Update an existing participant with given code
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can update a participant
		/// - Username must be unique
		/// - A valid participant code must be provided (currently just non-empty)
		/// - The participant allocation to sites is not changed by this call. Use PUT participants/{participantId}/assignToSite/{siteId} or participants/{participantId}/endFromSite/{siteId} instead
		/// - Request an empty user name or password, or a zero therapist ID in order not to change them
		/// </remarks>
		/// <param name="code"></param>
		/// <param name="participantUpdateRequest"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpPut("{code}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateParticipant(string code, [FromBody]ParticipantUpdateRequest participantUpdateRequest) {
			if (participantUpdateRequest == null) {
				_logger.LogError("UpdateParticipant: ParticipantUpdateRequest object sent from client is null.");
				return BadRequest("ParticipantUpdateRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateParticipant: Invalid ParticipantUpdateRequest object sent from client.");
				return BadRequest("Invalid ParticipantUpdateRequest object");
			}

			#region Participant checks
			var participant = await _coadaptService.Participant.GetParticipantByCodeAsync(code);
			if (participant.IsEmptyObject()) {
				_logger.LogError("UpdateParticipant: Participant with requested Code does not exist.");
				return NotFound("Participant with requested Code does not exist");
			}
			var user = await _userManager.FindByIdAsync(participant.UserId);
			if (!string.IsNullOrEmpty(participantUpdateRequest.Password)) {
				var passwordValidator = new PasswordValidator<IdentityUser>();
				if (!(await passwordValidator.ValidateAsync(_userManager, null, participantUpdateRequest.Password)).Succeeded) {
					_logger.LogError("UpdateParticipant: Provided password is not strong enough.");
					return BadRequest("Provided password is not strong enough");
				}
			}
			if (!string.IsNullOrEmpty(participantUpdateRequest.UserName)) {
				var dbUser = await _userManager.FindByNameAsync(participantUpdateRequest.UserName);
				if (dbUser != null && user.Id != dbUser.Id) {
					_logger.LogError("UpdateParticipant: username already exists.");
					return BadRequest("Username already exists");
				}
			}
			#endregion

			#region StudyParticipant checks
			if (participantUpdateRequest.StartDate > participantUpdateRequest.EndDate) {
				_logger.LogError("UpdateParticipant: The participation start date must be before the participation end date");
				return BadRequest("The participation start date must be before the participation end date");
			}
			//if studyparticipant exists, update it. If not, create it
			bool update = true;
			var site = await _coadaptService.Site.GetSiteByIdAsync(participantUpdateRequest.SiteId);
			var study = await _coadaptService.Study.GetStudyByIdAsync(site.StudyId);
			var studyParticipant = await _coadaptService.StudyParticipant.StudyParticipantByParticipantAndStudy(participant.Id, study.Id);
			if (studyParticipant.StudyId == 0) {
				//Participation does not exist. It will be created.
				update = false;
			} else {
				if (studyParticipant.Abandoned) {
					//Participation exists and has been abandoned. It will not be updated and an error will be returned.
					_logger.LogError("UpdateParticipant: Participation to this site/group pair cannot be updated because it has been abandoned.");
					return BadRequest("Participation to this site/group pair cannot be updated because it  has been abandoned");
				}
				//Participation exists. It will be updated.
			}

			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			IUserDetails requestingUser;
			if (role == Role.AdministratorRole) {
				requestingUser = await _coadaptService.Administrator.GetAdministratorByUserIdAsync(userId);
			} else if (role == Role.SubAdministratorRole) {
				requestingUser = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			} else {
				requestingUser = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			}
			if (site.IsEmptyObject()) {
				_logger.LogError("UpdateParticipant: The requested site does not exist.");
				return BadRequest("A requested site does not exist");
			}
			var group = await _coadaptService.Group.GetGroupByIdAsync(participantUpdateRequest.GroupId);
			if (group.IsEmptyObject()) {
				_logger.LogError("UpdateParticipant: The requested group does not exist.");
				return BadRequest("A requested group does not exist");
			}
			if (site.StudyId != group.StudyId) {
				_logger.LogError("UpdateParticipant: The requested site/group pair belongs to different studies.");
				return BadRequest("The requested site/group pair belongs to different studies");
			}
			var organization = await _coadaptService.Organization.GetOrganizationByIdAsync(study.OrganizationId);
			if (role == Role.SubAdministratorRole && organization.SubAdministratorId != requestingUser.Id) {
				_logger.LogError("UpdateParticipant: The requested site/group pair belongs to study outside the sub-administrator's organization.");
				return BadRequest("The requested site/group pair belongs to study outside the sub-administrator's organization");
			}
			if (role == Role.SupervisorRole && study.SupervisorId != requestingUser.Id) {
				_logger.LogError("UpdateParticipant: The requested site/group pair belongs to study supervised by a different supervisor.");
				return BadRequest("The requested site/group pair belongs to study supervised by a different supervisor");
			}
			#endregion

			#region Participant update
			int? therapistId = null;
			if (participantUpdateRequest.TherapistId == 0 || participantUpdateRequest.TherapistId == null) {
				therapistId = null;
			} else {
				var therapist = await _coadaptService.Therapist.GetTherapistByIdAsync((int)participantUpdateRequest.TherapistId);
				if (therapist.IsEmptyObject()) {
					therapistId = participant.TherapistId;
				} else {
					therapistId = therapist.Id;
				}
			}

			_coadaptService.Participant.UpdateParticipant(participant, new Participant() { 
				UserId = participant.UserId, 
				Code = code, 
				TherapistId = therapistId,
				Gender = participantUpdateRequest.Gender ?? participant.Gender,
				DateOfBirth = participantUpdateRequest.DateOfBirth ?? participant.DateOfBirth,
				DateOfFirstJob = participantUpdateRequest.DateOfFirstJob ?? participant.DateOfFirstJob
			});
			#endregion

			#region Studyparticipant create or update (depending on the update flag)
			if (update) {
				_coadaptService.StudyParticipant.UpdateStudyParticipant(studyParticipant, new StudyParticipant() {
					StudyId = study.Id,
					SiteId = site.Id,
					GroupId = group.Id,
					ParticipantId = participant.Id,
					StartDate = participantUpdateRequest.StartDate ?? studyParticipant.StartDate,
					EndDate = participantUpdateRequest.EndDate ?? studyParticipant.EndDate,
					DateOfCurrentJob = participantUpdateRequest.DateOfCurrentJob ?? studyParticipant.DateOfCurrentJob,
					Education = !string.IsNullOrWhiteSpace(participantUpdateRequest.Education) ? participantUpdateRequest.Education : studyParticipant.Education,
					JobType = !string.IsNullOrWhiteSpace(participantUpdateRequest.JobType) ? participantUpdateRequest.JobType : studyParticipant.JobType,
					MaritalStatus = !string.IsNullOrWhiteSpace(participantUpdateRequest.MaritalStatus) ? participantUpdateRequest.MaritalStatus : studyParticipant.MaritalStatus,
					RegionOfResidence = !string.IsNullOrWhiteSpace(participantUpdateRequest.Region) ? participantUpdateRequest.Region : studyParticipant.RegionOfResidence,
					Abandoned = studyParticipant.Abandoned
				});
			} else {
				_coadaptService.StudyParticipant.CreateStudyParticipant(new StudyParticipant() {
					StudyId = study.Id,
					SiteId = site.Id,
					GroupId = group.Id,
					ParticipantId = participant.Id,
					StartDate = participantUpdateRequest.StartDate ?? new DateTime(2000, 1, 1),
					EndDate = participantUpdateRequest.EndDate,
					DateOfCurrentJob = participantUpdateRequest.DateOfCurrentJob ?? new DateTime(2000, 1, 1),
					Education = participantUpdateRequest.Education,
					JobType = participantUpdateRequest.JobType,
					MaritalStatus = participantUpdateRequest.MaritalStatus,
					RegionOfResidence = participantUpdateRequest.Region
				});
			}
			#endregion

			await _coadaptService.SaveAsync();
			if (participantUpdateRequest.UserName != null && participantUpdateRequest.Password != null) {
				if (participantUpdateRequest.UserName.Length == 0 && participantUpdateRequest.Password.Length == 0) {
					return NoContent();
				}
			}
			if (participantUpdateRequest.UserName != null) {
				if (participantUpdateRequest.UserName.Length != 0) {
					await _userManager.SetUserNameAsync(user, participantUpdateRequest.UserName);
				}
			}
			if (participantUpdateRequest.Password != null) {
				if (participantUpdateRequest.Password.Length != 0) {
					var token = await _userManager.GeneratePasswordResetTokenAsync(user);
					await _userManager.ResetPasswordAsync(user, token, participantUpdateRequest.Password);
				}
			}
			return NoContent();
		}

		/// <summary>
		///	Retrieve the currently logged-in participant
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only participants retrieve self
		/// </remarks>
		[HttpGet("self")]
		[ProducesResponseType(typeof(Participant), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetLoggedinParticipant() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var participant = await _coadaptService.Participant.GetParticipantByUserIdAsync(userId);
			if (participant.IsEmptyObject()) {
				_logger.LogWarn($"GetLoggedinParticipant: Currently logged in user is not a participant!");
				return BadRequest("Currently logged in user is not a participant");
			}
			participant.User = await _userManager.FindByIdAsync(participant.UserId);
			return Ok(participant);
		}

		/// <summary>
		/// Update user part of currently logged in participant
		/// </summary>
		/// <remarks>
		/// - Only participants update self
		/// - Username must be unique
		/// - Request an empty user name or password in order not to change them
		/// </remarks>
		/// <param name="userRequest"></param>
		[HttpPut("self")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateLoggedinParticipant([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("UpdateLoggedinParticipant: UserRequest object sent from client is null.");
				return BadRequest("UserRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateLoggedinParticipant: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var participant = await _coadaptService.Participant.GetParticipantByUserIdAsync(userId);
			if (participant.IsEmptyObject()) {
				_logger.LogWarn($"UpdateLoggedinParticipant: Currently logged in user is not a participant!");
				return BadRequest("Currently logged in user is not a participant");
			}
			if (userRequest.UserName.Length == 0 && userRequest.Password.Length == 0) {
				return NoContent();
			}
			var user = await _userManager.FindByIdAsync(participant.UserId);
			if (userRequest.UserName.Length != 0) {
				var dbUser = await _userManager.FindByNameAsync(userRequest.UserName);
				if (dbUser != null && user.Id != dbUser.Id) {
					_logger.LogError("UpdateLoggedinParticipant: Username already exists.");
					return BadRequest("Username already exists");
				}
				await _userManager.SetUserNameAsync(user, userRequest.UserName);
			}
			if (userRequest.Password.Length != 0) {
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				await _userManager.ResetPasswordAsync(user, token, userRequest.Password);
			}
			return NoContent();
		}

	}
}
