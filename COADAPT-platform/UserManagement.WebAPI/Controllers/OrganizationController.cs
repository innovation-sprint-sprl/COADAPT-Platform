using Constants;
using Contracts.Logger;
using Contracts.Repository;
using Entities.Extensions;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ApiModels;

namespace UserManagement.WebAPI.Controllers {

	[Route("1.0/organizations")]
	[ApiController]
	[Authorize(Policy = "SubAdministrator")]
	public class OrganizationController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;
		private readonly UserManager<IdentityUser> _userManager;

		public OrganizationController(ILoggerManager logger, IRepositoryWrapper repository, UserManager<IdentityUser> userManager) {
			_logger = logger;
			_coadaptService = repository;
			_userManager = userManager;
		}

		/// <summary>
		/// Retrieve all organizations
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can retrieve all organizations
		/// </remarks>
		[Authorize(Policy = "Administrator")]
		[HttpGet]
		[ProducesResponseType(typeof(OrganizationListResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllOrganizations() {
			return Ok(await _coadaptService.Organization.GetAllOrganizationsAsync());
		}

		/// <summary>
		///	Retrieve the organization with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators and sub-administrators can retrieve an organization
		/// -- Administrators retrieve any organization
		/// -- Sub-administrators retrieve only own organization
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "Administrator")]
		[HttpGet("{id}", Name = "OrganizationById")]
		[ProducesResponseType(typeof(Organization), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetOrganizationById(int id) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organizationOfSubAdministrator = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organizationOfSubAdministrator.IsEmptyObject()) {
					_logger.LogWarn($"GetOrganizationById: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (organizationOfSubAdministrator.Id != id) {
					_logger.LogWarn($"GetOrganizationById: Requested organization is not owned by currently logged in sub-administrator!");
					return BadRequest("Requested organization is not owned by currently logged in sub-administrator");
				}
			}
			var organization = await _coadaptService.Organization.GetOrganizationByIdAsync(id);
			if (organization.IsEmptyObject()) {
				_logger.LogWarn($"GetOrganizationById: Organization with ID {id} not found!");
				return NotFound("Organization with requested ID does not exist");
			}
			organization.SubAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(organization.SubAdministratorId);
			organization.SubAdministrator.User = await _userManager.FindByIdAsync(organization.SubAdministrator.UserId);
			return Ok(organization);
		}

		/// <summary>
		///	Retrieve the organization with given short name
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators and sub-administrators can retrieve an organization
		/// -- Administrators retrieve any organization
		/// -- Sub-administrators retrieve only own organization
		/// </remarks>
		/// <param name="shortName"></param>
		// [Authorize(Policy = "Administrator")]
		// [HttpGet("{shortName}", Name = "OrganizationByShortName")]
		// [ProducesResponseType(typeof(Organization), StatusCodes.Status200OK)]
		// [ProducesResponseType(StatusCodes.Status400BadRequest)]
		// [ProducesResponseType(StatusCodes.Status404NotFound)]
		// [ProducesResponseType(StatusCodes.Status500InternalServerError)]
		private async Task<IActionResult> GetOrganizationByShortName(string shortName) {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organizationOfSubAdministrator = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organizationOfSubAdministrator.IsEmptyObject()) {
					_logger.LogWarn($"GetOrganizationById: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (organizationOfSubAdministrator.Shortname != shortName) {
					_logger.LogWarn($"GetOrganizationById: Requested organization is not owned by currently logged in sub-administrator!");
					return BadRequest("Requested organization is not owned by currently logged in sub-administrator");
				}
			}
			var organization = await _coadaptService.Organization.GetOrganizationByShortnameAsync(shortName);
			if (organization.IsEmptyObject()) {
				_logger.LogWarn($"GetOrganizationById: Organization with short name {shortName} not found!");
				return NotFound("Organization with requested short name does not exist");
			}
			organization.SubAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(organization.SubAdministratorId);
			organization.SubAdministrator.User = await _userManager.FindByIdAsync(organization.SubAdministrator.UserId);
			return Ok(organization);
		}

		/// <summary>
		///	Retrieve the organization of the currently logged-in sub-administrator
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only sub-administrators can retrieve own organization
		/// </remarks>
		[HttpGet("own")]
		[ProducesResponseType(typeof(Organization), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetOwnOrganization() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			if (subAdministrator.IsEmptyObject()) {
				_logger.LogError($"GetOwnOrganization: Currently logged in user is not a sub-administrator!");
				return BadRequest("Currently logged in user is not a sub-administrator");
			}
			var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
			if (organization.IsEmptyObject()) {
				_logger.LogWarn($"GetOwnOrganization: Currently logged in sub-administrator is not assigned to any organization!");
				return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
			}
			organization.SubAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(organization.SubAdministratorId);
			organization.SubAdministrator.User = await _userManager.FindByIdAsync(organization.SubAdministrator.UserId);
			return Ok(organization);
		}

		/// <summary>
		/// Create a new organization
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can create a new organization
		/// - Organization name cannot be empty
		/// - Organization short name cannot be empty and must be unique amongst organizations
		/// - The organization's sub-administrator is assigned at organization creation; This sub-administrator cannot be already assigned to another organization
		/// - Organizations are created without studies; studies are assigned to organizations at study creation
		/// </remarks>
		/// <param name="organizationRequest"></param>
		[Authorize(Policy = "Administrator")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateOrganization([FromBody]OrganizationRequest organizationRequest) {
			if (organizationRequest == null) {
				_logger.LogError("CreateOrganization: OrganizationRequest object sent from client is null.");
				return BadRequest("OrganizationRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateOrganization: Invalid OrganizationRequest object sent from client.");
				return BadRequest("Invalid OrganizationRequest object");
			}
			if (organizationRequest.Name == "") {
				_logger.LogError("CreateOrganization: Organization name cannot be empty.");
				return BadRequest("Organization name cannot be empty");
			}
			if (organizationRequest.Shortname == "") {
				_logger.LogError("CreateOrganization: Organization short name cannot be empty.");
				return BadRequest("Organization short name cannot be empty");
			}
			var organizationOfSameShortname = await _coadaptService.Organization.GetOrganizationByShortnameAsync(organizationRequest.Shortname);
			if (!organizationOfSameShortname.IsEmptyObject()) {
				_logger.LogWarn($"CreateOrganization: Organization with same short name already exists!");
				return BadRequest("Organization with same short name already exists");
			}
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(organizationRequest.SubAdministratorId);
			if (subAdministrator.IsEmptyObject()) {
				_logger.LogWarn($"CreateOrganization: Sub-administrator with ID {organizationRequest.SubAdministratorId} not found!");
				return BadRequest("Sub-administrator with requested ID does not exist");
			}
			var organization = new Organization { Name = organizationRequest.Name, Shortname = organizationRequest.Shortname, SubAdministratorId = organizationRequest.SubAdministratorId };
			var organizationOfSubAdministrator = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(organization.SubAdministratorId);
			if (!organizationOfSubAdministrator.IsEmptyObject()) {
				_logger.LogWarn("CreateOrganization: Specified sub-administrator already assigned to another organization.");
				return BadRequest("Specified sub-administrator already assigned to another organization");
			}
			_coadaptService.Organization.CreateOrganization(organization);
			await _coadaptService.SaveAsync();
			organization.SubAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(organization.SubAdministratorId);
			organization.SubAdministrator.User = await _userManager.FindByIdAsync(organization.SubAdministrator.UserId);
			return CreatedAtRoute("OrganizationById", new { id = organization.Id }, organization);
		}

		/// <summary>
		/// Delete the organization with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can delete an organization
		/// - Cannot delete organization that has studies; reassign or delete the studies first
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "Administrator")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteOrganization(int id) {
			var organization = await _coadaptService.Organization.GetOrganizationByIdAsync(id);
			if (organization.IsEmptyObject()) {
				_logger.LogWarn($"DeleteOrganization: Organization with ID {id} not found.");
				return NotFound("Organization with requested ID does not exist");
			}
			if (await _coadaptService.Study.CountStudiesByOrganizationIdAsync(id) > 0) {
				_logger.LogError($"DeleteOrganization: Cannot delete organization that has studies.");
				return BadRequest("Cannot delete organization that has studies");
			}
			_coadaptService.Organization.DeleteOrganization(organization);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Update an existing organization with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators and sub-administrators can update an organization
		/// -- Administrators update any organization
		/// -- Sub-administrators update only own organization and cannot reassign it to another sub-administrator; sub-administrator ID is ignored
		/// - Request an empty name or short name, or zero sub-administrator ID in order not to change that item
		/// - Organization short name must be unique amongst organizations
		/// - The requested sub-administrator cannot be already assigned to another organization
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="organizationRequest"></param>
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateOrganization(int id, [FromBody]OrganizationRequest organizationRequest) {
			if (organizationRequest == null) {
				_logger.LogError("UpdateOrganization: OrganizationRequest object sent from client is null.");
				return BadRequest("OrganizationRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateOrganization: Invalid OrganizationRequest object sent from client.");
				return BadRequest("Invalid OrganizationRequest object");
			}
			var dbOrganization = await _coadaptService.Organization.GetOrganizationByIdAsync(id);
			if (dbOrganization.IsEmptyObject()) {
				_logger.LogWarn($"UpdateOrganization: Organization with ID {id} not found.");
				return NotFound("Organization with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organizationOfSubAdministrator = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organizationOfSubAdministrator.IsEmptyObject()) {
					_logger.LogWarn($"UpdateOrganization: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (organizationOfSubAdministrator.Id != id) {
					_logger.LogWarn($"UpdateOrganization: Requested organization is not owned by currently logged in sub-administrator!");
					return BadRequest("Requested organization is not owned by currently logged in sub-administrator");
				}
			}
			if (organizationRequest.Shortname == "") {
				organizationRequest.Shortname = dbOrganization.Shortname;
			}
			if (organizationRequest.Shortname != dbOrganization.Shortname) {
				var organizationOfSameShortname = await _coadaptService.Organization.GetOrganizationByShortnameAsync(organizationRequest.Shortname);
				if (!organizationOfSameShortname.IsEmptyObject()) {
					_logger.LogWarn($"UpdateOrganization: Organization with same short name already exists!");
					return BadRequest("Organization with same short name already exists");
				}
			}
			if (organizationRequest.SubAdministratorId == 0 || role == Role.SubAdministratorRole) {
				organizationRequest.SubAdministratorId = dbOrganization.SubAdministratorId;
			} else {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(organizationRequest.SubAdministratorId);
				if (subAdministrator.IsEmptyObject()) {
					_logger.LogWarn($"UpdateOrganization: Sub-administrator with ID {organizationRequest.SubAdministratorId} not found!");
					return BadRequest("Sub-administrator with requested ID does not exist");
				}
				var organizationOfSubAdministrator = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(organizationRequest.SubAdministratorId);
				if (!organizationOfSubAdministrator.IsEmptyObject()) {
					_logger.LogWarn("UpdateOrganization: Specified sub-administrator already assigned to another organization.");
					return BadRequest("Specified sub-administrator already assigned to another organization");
				}
			}
			if (organizationRequest.Name == "") {
				organizationRequest.Name = dbOrganization.Name;
			}
			var organization = new Organization { Name = organizationRequest.Name, Shortname = organizationRequest.Shortname, SubAdministratorId = organizationRequest.SubAdministratorId };
			_coadaptService.Organization.UpdateOrganization(dbOrganization, organization);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Update the organization of the currently logged-in sub-administrator
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only sub-administrators can update own organization
		/// - Request an empty name or short name in order not to change that item
		/// - The sub-administrator cannot be changed, thus any provided ID is ignored
		/// - Organization short name must be unique amongst organizations
		/// </remarks>
		/// <param name="organizationRequest"></param>
		[HttpPut("own")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateOwnOrganization([FromBody]OrganizationRequest organizationRequest) {
			if (organizationRequest == null) {
				_logger.LogError("UpdateOwnOrganization: OrganizationRequest object sent from client is null.");
				return BadRequest("OrganizationRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateOwnOrganization: Invalid OrganizationRequest object sent from client.");
				return BadRequest("Invalid OrganizationRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			if (subAdministrator.IsEmptyObject()) {
				_logger.LogWarn($"UpdateOwnOrganization: Currently logged in user is not a sub-administrator!");
				return BadRequest("Currently logged in user is not a sub-administrator");
			}
			var dbOrganization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
			if (dbOrganization.IsEmptyObject()) {
				_logger.LogWarn($"UpdateOwnOrganization: Currently logged in sub-administrator is not assigned to any organization!");
				return NotFound("Currently logged in sub-administrator is not assigned to any organization");
			}
			if (organizationRequest.Shortname == "") {
				organizationRequest.Shortname = dbOrganization.Shortname;
			}
			if (organizationRequest.Shortname != dbOrganization.Shortname) {
				var organizationOfSameShortname = await _coadaptService.Organization.GetOrganizationByShortnameAsync(organizationRequest.Shortname);
				if (!organizationOfSameShortname.IsEmptyObject()) {
					_logger.LogWarn($"UpdateOwnOrganization: Organization with same short name already exists!");
					return BadRequest("Organization with same short name already exists");
				}
			}
			organizationRequest.SubAdministratorId = subAdministrator.Id;
			if (organizationRequest.Name == "") {
				organizationRequest.Name = dbOrganization.Name;
			}
			var organization = new Organization { Name = organizationRequest.Name, Shortname = organizationRequest.Shortname, SubAdministratorId = organizationRequest.SubAdministratorId };
			_coadaptService.Organization.UpdateOrganization(dbOrganization, organization);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

	}
}
