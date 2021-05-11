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
using Microsoft.AspNetCore.Mvc;
using ApiModels;

namespace UserManagement.WebAPI.Controllers {

	[Route("1.0/sites")]
	[ApiController]
	[Authorize(Policy = "Supervisor")]
	public class SiteController : ControllerBase {

		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;

		public SiteController(ILoggerManager logger, IRepositoryWrapper repository) {
			_logger = logger;
			_coadaptService = repository;
		}

		/// <summary>
		/// Retrieves all sites
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve sites
		/// -- An administrator retrieves all sites
		/// -- A sub-administrator retrieves all sites of own organization
		/// -- A supervisor retrieves only sites of own studies
		/// </remarks>
		[HttpGet]
		[ProducesResponseType(typeof(SiteListResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllSites() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.AdministratorRole) {
				return Ok(await _coadaptService.Site.GetAllSitesAsync());
			}
			var sites = new List<SiteListResponse>();
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetAllSites: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				var studiesOfOrganization = await _coadaptService.Study.StudiesByOrganization(organization.Id);
				foreach (var study in studiesOfOrganization) {
					sites.AddRange(await _coadaptService.Site.SiteListByStudy(study.Id));
				}
				return Ok(sites);
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			var studiesOfSupervisor = await _coadaptService.Study.GetStudiesBySupervisorIdAsync(supervisor.Id);
			foreach (var study in studiesOfSupervisor) {
				sites.AddRange(await _coadaptService.Site.SiteListByStudy(study.Id));
			}
			return Ok(sites);
		}

		/// <summary>
		///	Retrieve the site with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve a site
		/// -- An administrator retrieves any site
		/// -- A sub-administrator retrieves a site if it belongs to a study of own organization
		/// -- A supervisor retrieves a site if it belongs to own studies
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "SiteById")]
		[ProducesResponseType(typeof(Site), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetSiteById(int id) {
			var site = await _coadaptService.Site.GetSiteByIdAsync(id);
			if (site.IsEmptyObject()) {
				_logger.LogError($"GetSiteById: Site with ID {id} not found.");
				return NotFound("Site with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var study = await _coadaptService.Study.GetStudyByIdAsync(site.StudyId);
			if (role == Role.AdministratorRole) {
				site.Study = study;
				return Ok(site);
			}
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetSiteById: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId == organization.Id) {
					site.Study = study;
					return Ok(site);
				}
				_logger.LogWarn($"GetSiteById: Requested site does not belong of to a study of the organization of the currently logged in sub-administrator!");
				return BadRequest("Requested site does not belong of to a study of the organization of the currently logged in sub-administrator");
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (study.SupervisorId == supervisor.Id) {
				site.Study = study;
				return Ok(site);
			}
			_logger.LogWarn($"GetSiteById: Requested site does not belong to a study of the currently logged in supervisor!");
			return BadRequest("Requested site does not belong to a study of the currently logged in supervisor");
		}

		/// <summary>
		///	Retrieve the site with short name and parent study ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve a site
		/// -- An administrator retrieves any site
		/// -- A sub-administrator retrieves a site if it belongs to a study of own organization
		/// -- A supervisor retrieves a site if it belongs to own studies
		/// </remarks>
		/// <param name="shortName"></param>
		/// <param name="studyId"></param>
		[HttpGet("{shortName}/{studyId}", Name = "SiteByShortNameAndStudyId")]
		[ProducesResponseType(typeof(Site), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetSiteByShortNameAndStudyId(string shortName, int studyId) {
			var site = await _coadaptService.Site.GetSiteOfStudyByShortnameAsync(shortName, studyId);
			if (site.IsEmptyObject()) {
				_logger.LogError($"GetSiteByShortNameAndStudyId: Site with short name {shortName} not found in study with ID {studyId}.");
				return NotFound("Site with requested short name and parent study ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var study = await _coadaptService.Study.GetStudyByIdAsync(site.StudyId);
			if (role == Role.AdministratorRole) {
				site.Study = study;
				return Ok(site);
			}
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetSiteByShortNameAndStudyId: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId == organization.Id) {
					site.Study = study;
					return Ok(site);
				}
				_logger.LogWarn($"GetSiteByShortNameAndStudyId: Requested site does not belong of to a study of the organization of the currently logged in sub-administrator!");
				return BadRequest("Requested site does not belong of to a study of the organization of the currently logged in sub-administrator");
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (study.SupervisorId == supervisor.Id) {
				site.Study = study;
				return Ok(site);
			}
			_logger.LogWarn($"GetSiteByShortNameAndStudyId: Requested site does not belong to a study of the currently logged in supervisor!");
			return BadRequest("Requested site does not belong to a study of the currently logged in supervisor");
		}

		/// <summary>
		///	Retrieve the site with given path of short names [organization short name].[study short name].[site short name]
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve a site
		/// -- An administrator retrieves any site
		/// -- A sub-administrator retrieves a site if it belongs to a study of own organization
		/// -- A supervisor retrieves a site if it belongs to own studies
		/// </remarks>
		/// <param name="names"></param>
		[HttpGet("names/{names}", Name = "SiteByShortNamesPath")]
		[ProducesResponseType(typeof(Site), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetSiteByShortNamesPath(string names) {
			var nameComponents = names.Split(".");
			if (nameComponents.Length != 3) {
				_logger.LogWarn($"GetSiteByShortNamesPath: Expected three short names separated py periods: [organization short name].[study short name].[site short name]");
				return BadRequest("Expected three short names separated py periods: [organization short name].[study short name].[site short name]");
			}
			var organization = await _coadaptService.Organization.GetOrganizationByShortnameAsync(nameComponents[0]);
			if (organization.IsEmptyObject()) {
				_logger.LogWarn($"GetSiteByShortNamesPath: Organization with short name {nameComponents[0]} does not exist!");
				return BadRequest("Organization with requested short name does not exist");
			}
			var study = await _coadaptService.Study.GetStudyOfOrganizationByShortnameAsync(nameComponents[1], organization.Id);
			if (study.IsEmptyObject()) {
				_logger.LogWarn($"GetSiteByShortNamesPath: Study with short name {nameComponents[1]} does not exist in organization with short name {nameComponents[0]}!");
				return BadRequest("Study with requested short name does not exist in specified organization");
			}
			var site = await _coadaptService.Site.GetSiteOfStudyByShortnameAsync(nameComponents[2], study.Id);
			if (site.IsEmptyObject()) {
				_logger.LogError($"GetSiteByShortNamesPath: Site with short name {nameComponents[2]} not found in parent study.");
				return NotFound("Site with requested short name and parent study does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.AdministratorRole) {
				site.Study = study;
				return Ok(site);
			}
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organizationOfSubAdmin = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organizationOfSubAdmin.IsEmptyObject()) {
					_logger.LogWarn($"GetSiteByShortNamesPath: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId == organizationOfSubAdmin.Id) {
					site.Study = study;
					return Ok(site);
				}
				_logger.LogWarn($"GetSiteByShortNamesPath: Requested site does not belong of to a study of the organization of the currently logged in sub-administrator!");
				return BadRequest("Requested site does not belong of to a study of the organization of the currently logged in sub-administrator");
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (study.SupervisorId == supervisor.Id) {
				site.Study = study;
				return Ok(site);
			}
			_logger.LogWarn($"GetSiteByShortNamesPath: Requested site does not belong to a study of the currently logged in supervisor!");
			return BadRequest("Requested site does not belong to a study of the currently logged in supervisor");
		}

		/// <summary>
		/// Create a new site
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can create a site
		/// -- An administrator creates any site
		/// -- A sub-administrator creates a site belonging to a study of own organization
		/// -- A supervisor creates a site belonging to own studies
		/// - Site name cannot be empty
		/// - Site short name cannot be empty and must be unique amongst sites of the same study
		/// - The site is assigned to a valid study at creation
		/// - Studies are created without participants; participants are assigned to sites at participant creation or update
		/// </remarks>
		/// <param name="siteRequest"></param>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateSite([FromBody]SiteRequest siteRequest) {
			if (siteRequest == null) {
				_logger.LogError("CreateSite: SiteRequest object sent from client is null.");
				return BadRequest("SiteRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateSite: Invalid SiteRequest object sent from client.");
				return BadRequest("Invalid SiteRequest object");
			}
			if (siteRequest.Name == "") {
				_logger.LogError("CreateSite: Site name cannot be empty.");
				return BadRequest("Site name cannot be empty");
			}
			if (siteRequest.Shortname == "") {
				_logger.LogError("CreateSite: Site short name cannot be empty.");
				return BadRequest("Site short name cannot be empty");
			}
			var study = await _coadaptService.Study.GetStudyByIdAsync(siteRequest.StudyId);
			if (study.IsEmptyObject()) {
				_logger.LogError($"CreateSite: Study with ID {siteRequest.StudyId} not found.");
				return BadRequest("Study with requested ID does not exist");
			}
			var siteOfSameShortname = await _coadaptService.Site.GetSiteOfStudyByShortnameAsync(siteRequest.Shortname, siteRequest.StudyId);
			if (!siteOfSameShortname.IsEmptyObject()) {
				_logger.LogError($"CreateSite: Site with same short name already exists in study!");
				return BadRequest("Site with same short name already exists in study");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"CreateSite: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId != organization.Id) {
					_logger.LogWarn($"CreateSite: Requested study does not belong to the organization of the currently logged in sub-administrator!");
					return BadRequest("Requested study does not belong to the organization of the currently logged in sub-administrator");
				}
			} else if (role == Role.SupervisorRole) {
				var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (study.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"CreateSite: Requested study does not belong to the currently logged in supervisor!");
					return BadRequest("Requested study does not belong to the currently logged in supervisor");
				}
			}
			var site = new Site { Name = siteRequest.Name, Shortname = siteRequest.Shortname, StudyId = siteRequest.StudyId };
			_coadaptService.Site.CreateSite(site);
			await _coadaptService.SaveAsync();
			site.Study = study;
			return CreatedAtRoute("SiteById", new { id = site.Id }, site);
		}

		/// <summary>
		/// Update an existing site with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can update a site
		/// -- An administrator updates any site
		/// -- A sub-administrator updates a site belonging to a study of own organization and cannot reassign it to a study of a different organization
		/// -- A supervisor creates a site belonging to own studies and cannot reassign it to a study other than those
		/// - Request an empty name or short name, or zero study ID in order not to change that item
		/// - Site short name must be unique amongst sites of study
		/// - The requested study ID (if not zero) must already be created
		/// - A site with participants cannot be moved to a different study
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="siteRequest"></param>
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateSite(int id, [FromBody]SiteRequest siteRequest) {
			if (siteRequest == null) {
				_logger.LogError("UpdateSite: SiteRequest object sent from client is null.");
				return BadRequest("SiteRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateSite: Invalid SiteRequest object sent from client.");
				return BadRequest("Invalid SiteRequest object");
			}
			var dbSite = await _coadaptService.Site.GetSiteByIdAsync(id);
			if (dbSite.IsEmptyObject()) {
				_logger.LogError($"UpdateSite: Site with ID {id} not found.");
				return NotFound("Site with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			SubAdministrator subAdministrator = new SubAdministrator();
			Organization organization = new Organization();
			Supervisor supervisor = new Supervisor();
			var originalStudy = await _coadaptService.Study.GetStudyByIdAsync(dbSite.StudyId);
			if (role == Role.SubAdministratorRole) {
				subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"UpdateSite: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (originalStudy.OrganizationId != organization.Id) {
					_logger.LogWarn($"UpdateSite: Study of updated site does not belong to the organization of the currently logged in sub-administrator!");
					return BadRequest("Study of updated site does not belong to the organization of the currently logged in sub-administrator");
				}
			} else if (role == Role.SupervisorRole) {
				supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (originalStudy.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"UpdateSite: Study of updated site does not belong to the currently logged in supervisor!");
					return BadRequest("Study of updated site does not belong to the currently logged in supervisor");
				}
			}
			if (siteRequest.StudyId == 0) {
				siteRequest.StudyId = dbSite.StudyId;
			}
			if (siteRequest.StudyId != dbSite.StudyId) {
				var study = await _coadaptService.Study.GetStudyByIdAsync(siteRequest.StudyId);
				if (study.IsEmptyObject()) {
					_logger.LogError($"UpdateSite: Study with ID {siteRequest.StudyId} not found.");
					return BadRequest("Study with requested ID does not exist");
				}
				if (role == Role.SubAdministratorRole) {
					if (study.OrganizationId != organization.Id) {
						_logger.LogWarn($"UpdateSite: Requested study does not belong to the organization of the currently logged in sub-administrator!");
						return BadRequest("Requested study does not belong to the organization of the currently logged in sub-administrator");
					}
				} else if (role == Role.SupervisorRole) {
					if (study.SupervisorId != supervisor.Id) {
						_logger.LogWarn($"UpdateSite: Requested study does not belong to the currently logged in supervisor!");
						return BadRequest("Requested study does not belong to the currently logged in supervisor");
					}
				}
			}
			if (siteRequest.Shortname == "") {
				siteRequest.Shortname = dbSite.Shortname;
			}
			if (siteRequest.Shortname != dbSite.Shortname) {
				var siteOfSameShortname = await _coadaptService.Site.GetSiteOfStudyByShortnameAsync(siteRequest.Shortname, siteRequest.StudyId);
				if (!siteOfSameShortname.IsEmptyObject()) {
					_logger.LogError($"UpdateSite: Site with same short name already exists in study!");
					return BadRequest("Site with same short name already exists in study");
				}
			}
			if (siteRequest.Name == "") {
				siteRequest.Name = dbSite.Name;
			}
			var site = new Site { Name = siteRequest.Name, Shortname = siteRequest.Shortname, StudyId = siteRequest.StudyId };
			_coadaptService.Site.UpdateSite(dbSite, site);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Delete the site with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can delete a site
		/// -- An administrator deletes any site
		/// -- A sub-administrator deletes a site if it belongs to a study of own organization
		/// -- A supervisor deletes a site if it belongs to own studies
		/// - Cannot delete site that has participants; un-assign or delete the participants first
		/// </remarks>
		/// <param name="id"></param>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteSite(int id) {
			var site = await _coadaptService.Site.GetSiteByIdAsync(id);
			if (site.IsEmptyObject()) {
				_logger.LogError($"DeleteSite: Site with ID {id} not found.");
				return NotFound("Site with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var study = await _coadaptService.Study.GetStudyByIdAsync(site.StudyId);
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"DeleteSite: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId != organization.Id) {
					_logger.LogWarn($"DeleteSite: Study of site to delete does not belong to the organization of the currently logged in sub-administrator!");
					return BadRequest("Study of site to delete does not belong to the organization of the currently logged in sub-administrator");
				}
			} else if (role == Role.SupervisorRole) {
				var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (study.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"DeleteSite: Study of site to delete does not belong to the currently logged in supervisor!");
					return BadRequest("Study of site to delete does not belong to the currently logged in supervisor");
				}
			}
			if (((ICollection<StudyParticipant>)await _coadaptService.StudyParticipant.StudyParticipantsBySite(id, false)).Count > 0) {
				_logger.LogError($"DeleteSite: Cannot delete site with participants.");
				return BadRequest("Cannot delete site with participants");
			}
			_coadaptService.Site.DeleteSite(site);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

	}

}
