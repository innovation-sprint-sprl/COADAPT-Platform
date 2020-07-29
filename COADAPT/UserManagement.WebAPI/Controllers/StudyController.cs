using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants;
using Contracts.Logger;
using Contracts.Repository;
using Entities.Extensions;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApiModels;

namespace UserManagement.WebAPI.Controllers {

	[Route("1.0/studies")]
	[ApiController]
	[Authorize(Policy = "Supervisor")]
	public class StudyController : ControllerBase {

		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;
		private readonly UserManager<IdentityUser> _userManager;

		public StudyController(ILoggerManager logger, IRepositoryWrapper coadaptService, UserManager<IdentityUser> userManager) {
			_logger = logger;
			_coadaptService = coadaptService;
			_userManager = userManager;
		}

		/// <summary>
		/// Retrieves all studies
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve studies
		/// -- An administrator retrieves all studies
		/// -- A sub-administrator retrieves all studies of own organization
		/// -- A supervisor retrieves only own studies
		/// </remarks>
		[HttpGet]
		[ProducesResponseType(typeof(Study), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllStudies() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.AdministratorRole) {
				return Ok(await _coadaptService.Study.GetAllStudiesAsync());
			}
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetAllStudies: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				return Ok(await _coadaptService.Study.StudiesByOrganization(organization.Id));
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (supervisor.IsEmptyObject()) {
				_logger.LogWarn($"GetAllStudies: Currently logged in user is not an administrator, sub-administrator or supervisor!");
				return BadRequest("Currently logged in user is not an administrator, sub-administrator or supervisor");
			}
			return Ok(await _coadaptService.Study.GetStudiesBySupervisorIdAsync(supervisor.Id));
		}

		/// <summary>
		///	Retrieve the study with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve a study
		/// -- An administrator retrieves any study
		/// -- A sub-administrator retrieves the study only if it belongs to own organization
		/// -- A supervisor retrieves the study only if it is one of own studies
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "StudyById")]
		[ProducesResponseType(typeof(Study), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetStudyById(int id) {
			var study = await _coadaptService.Study.GetStudyByIdAsync(id);
			if (study.IsEmptyObject()) {
				_logger.LogError($"GetStudyById: Study with ID {id} not found.");
				return NotFound("Study with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetStudyById: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (organization.Id != study.OrganizationId) {
					_logger.LogWarn($"GetStudyById: Requested study is not part of the organization of currently logged in sub-administrator!");
					return BadRequest("Requested study is not part of the organization of currently logged in sub-administrator");
				}
			}
			if (role == Role.SupervisorRole) {
				var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (study.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"GetStudyById: Requested study is not supervised by currently logged in supervisor!");
					return BadRequest("Requested study is not supervised by currently logged in supervisor");
				}
			}
			study.Supervisor = await _coadaptService.Supervisor.GetSupervisorByIdAsync(study.SupervisorId);
			study.Supervisor.User = await _userManager.FindByIdAsync(study.Supervisor.UserId);
			study.Organization = await _coadaptService.Organization.GetOrganizationByIdAsync(study.OrganizationId);
			study.Sites = (ICollection<Site>)await _coadaptService.Site.SitesByStudy(id);
			return Ok(study);
		}

		/// <summary>
		/// Create a new study
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators and sub-administrators can create studies
		/// -- Organization ID is ignored if logged-in user is a sub-administrator; own organization is used instead
		/// - Study name cannot be empty
		/// - Study short name cannot be empty and must be unique amongst studies of the same organization
		/// - The study's supervisor is assigned at study creation
		/// - Studies are created without sites; sites are assigned to studies at site creation
		/// </remarks>
		/// <param name="studyRequest"></param>
		[Authorize(Policy = "SubAdministrator")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateStudy([FromBody]StudyRequest studyRequest) {
			if (studyRequest == null) {
				_logger.LogError("CreateStudy: StudyRequest object sent from client is null.");
				return BadRequest("StudyRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateStudy: Invalid StudyRequest object sent from client.");
				return BadRequest("Invalid StudyRequest object");
			}
			if (studyRequest.Name == "") {
				_logger.LogError("CreateStudy: Study name cannot be empty.");
				return BadRequest("Study name cannot be empty");
			}
			if (studyRequest.Shortname == "") {
				_logger.LogError("CreateStudy: Study short name cannot be empty.");
				return BadRequest("Study short name cannot be empty");
			}
			var studyOfSameShortname = await _coadaptService.Study.GetStudyOfOrganizationByShortnameAsync(studyRequest.Shortname, studyRequest.OrganizationId);
			if (!studyOfSameShortname.IsEmptyObject()) {
				_logger.LogError($"CreateStudy: Study with same short name already exists in organization!");
				return BadRequest("Study with same short name already exists in organization");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var subAdministratorOrganization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (subAdministratorOrganization.IsEmptyObject()) {
					_logger.LogWarn($"CreateStudy: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				studyRequest.OrganizationId = subAdministratorOrganization.Id;
			}
			var organization = await _coadaptService.Organization.GetOrganizationByIdAsync(studyRequest.OrganizationId);
			if (organization.IsEmptyObject()) {
				_logger.LogError($"CreateStudy: Organization with ID {studyRequest.OrganizationId} not found.");
				return BadRequest("Organization with requested ID does not exist");
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByIdAsync(studyRequest.SupervisorId);
			if (supervisor.IsEmptyObject()) {
				_logger.LogError($"CreateStudy: Supervisor with ID {studyRequest.SupervisorId} not found.");
				return BadRequest("Supervisor with requested ID does not exist");
			}
			var study = new Study { Name = studyRequest.Name, Shortname = studyRequest.Shortname, OrganizationId = studyRequest.OrganizationId, SupervisorId = studyRequest.SupervisorId };
			_coadaptService.Study.CreateStudy(study);
			await _coadaptService.SaveAsync();
			study.Supervisor = supervisor;
			study.Supervisor.User = await _userManager.FindByIdAsync(study.Supervisor.UserId);
			study.Organization = organization;
			return CreatedAtRoute("StudyById", new { id = study.Id }, study);
		}

		/// <summary>
		/// Update an existing study with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can update a study
		/// -- An administrator updates all information of any study
		/// -- A sub-administrator updates the study only if it belongs to own organization and cannot change its organization; the organization ID is ignored
		/// -- A supervisor updates the study only if it is one of own studies and cannot change its organization or supervisor; the organization and supervisor IDs are ignored
		/// - Request an empty name or short name, or zero supervisor or organization ID in order not to change that item
		/// - Study short name must be unique amongst studies of organization
		/// - The requested organization and supervisor IDs (if not zero) must already be created
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="studyRequest"></param>
		[Authorize(Policy = "SubAdministrator")]
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateStudy(int id, [FromBody]StudyRequest studyRequest) {
			if (studyRequest == null) {
				_logger.LogError("UpdateStudy: StudyRequest object sent from client is null.");
				return BadRequest("StudyRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateStudy: Invalid StudyRequest object sent from client.");
				return BadRequest("Invalid StudyRequest object");
			}
			var dbStudy = await _coadaptService.Study.GetStudyByIdAsync(id);
			if (dbStudy.IsEmptyObject()) {
				_logger.LogError($"UpdateStudy: Study with ID {id} not found.");
				return NotFound("Study with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"UpdateStudy: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (organization.Id != dbStudy.OrganizationId) {
					_logger.LogWarn($"UpdateStudy: Requested study is not part of the organization of currently logged in sub-administrator!");
					return BadRequest("Requested study is not part of the organization of currently logged in sub-administrator");
				}
			}
			if (role == Role.SupervisorRole) {
				var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (dbStudy.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"UpdateStudy: Requested study is not supervised by currently logged in supervisor!");
					return BadRequest("Requested study is not supervised by currently logged in supervisor");
				}
			}
			if (studyRequest.OrganizationId == 0 || role != Role.AdministratorRole) {
				studyRequest.OrganizationId = dbStudy.OrganizationId;
			}
			if (studyRequest.OrganizationId != dbStudy.OrganizationId) {
				var organization = await _coadaptService.Organization.GetOrganizationByIdAsync(studyRequest.OrganizationId);
				if (organization.IsEmptyObject()) {
					_logger.LogError($"UpdateStudy: Organization with ID {studyRequest.OrganizationId} not found.");
					return BadRequest("Organization with requested ID does not exist");
				}
			}
			if (studyRequest.SupervisorId == 0 || (role != Role.AdministratorRole && role != Role.SubAdministratorRole)) {
				studyRequest.SupervisorId = dbStudy.SupervisorId;
			}
			if (studyRequest.SupervisorId != dbStudy.SupervisorId) {
				var supervisor = await _coadaptService.Supervisor.GetSupervisorByIdAsync(studyRequest.SupervisorId);
				if (supervisor.IsEmptyObject()) {
					_logger.LogError($"UpdateStudy: Supervisor with ID {studyRequest.SupervisorId} not found.");
					return BadRequest("Supervisor with requested ID does not exist");
				}
			}
			if (studyRequest.Shortname == "") {
				studyRequest.Shortname = dbStudy.Shortname;
			}
			if (studyRequest.Shortname != dbStudy.Shortname) {
				var studyOfSameShortname = await _coadaptService.Study.GetStudyOfOrganizationByShortnameAsync(studyRequest.Shortname, studyRequest.OrganizationId);
				if (!studyOfSameShortname.IsEmptyObject()) {
					_logger.LogError($"UpdateStudy: Study with same short name already exists in organization!");
					return BadRequest("Study with same short name already exists in organization");
				}
			}
			if (studyRequest.Name == "") {
				studyRequest.Name = dbStudy.Name;
			}
			var study = new Study { Name = studyRequest.Name, Shortname = studyRequest.Shortname, OrganizationId = studyRequest.OrganizationId, SupervisorId = studyRequest.SupervisorId };
			_coadaptService.Study.UpdateStudy(dbStudy, study);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Delete the study with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can delete a study
		/// -- An administrator deletes any study
		/// -- A sub-administrator deletes the study only if it belongs to own organization
		/// -- A supervisor deletes the study only if it is one of own studies
		/// - Cannot delete study that has sites; reassign or delete the sites first
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "SubAdministrator")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteStudy(int id) {
			var study = await _coadaptService.Study.GetStudyByIdAsync(id);
			if (study.IsEmptyObject()) {
				_logger.LogError($"DeleteStudy: Study with ID {id} not found.");
				return NotFound("Study with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"DeleteStudy: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (organization.Id != study.OrganizationId) {
					_logger.LogWarn($"DeleteStudy: Requested study is not part of the organization of currently logged in sub-administrator!");
					return BadRequest("Requested study is not part of the organization of currently logged in sub-administrator");
				}
			}
			if (role == Role.SupervisorRole) {
				var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (study.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"DeleteStudy: Requested study is not supervised by currently logged in supervisor!");
					return BadRequest("Requested study is not supervised by currently logged in supervisor");
				}
			}
			if (await _coadaptService.Site.CountSitesByStudy(id) > 0) {
				_logger.LogError($"DeleteStudy: Cannot delete study thet has sites.");
				return BadRequest("Cannot delete study thet has sites");
			}
			_coadaptService.Study.DeleteStudy(study);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

	}

}
