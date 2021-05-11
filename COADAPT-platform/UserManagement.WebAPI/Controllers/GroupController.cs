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

	[Route("1.0/groups")]
	[ApiController]
	[Authorize(Policy = "Supervisor")]
	public class GroupController : ControllerBase {

		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;

		public GroupController(ILoggerManager logger, IRepositoryWrapper repository) {
			_logger = logger;
			_coadaptService = repository;
		}

		/// <summary>
		/// Retrieves all groups
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve groups
		/// -- An administrator retrieves all groups
		/// -- A sub-administrator retrieves all groups of own organization
		/// -- A supervisor retrieves only groups of own studies
		/// </remarks>
		[HttpGet]
		[ProducesResponseType(typeof(GroupListResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllGroups() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.AdministratorRole) {
				return Ok(await _coadaptService.Group.GetAllGroupsAsync());
			}
			var groups = new List<GroupListResponse>();
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetAllSites: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				var studiesOfOrganization = await _coadaptService.Study.StudiesByOrganization(organization.Id);
				foreach (var study in studiesOfOrganization) {
					groups.AddRange(await _coadaptService.Group.GroupListByStudy(study.Id));
				}
				return Ok(groups);
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			var studiesOfSupervisor = await _coadaptService.Study.GetStudiesBySupervisorIdAsync(supervisor.Id);
			foreach (var study in studiesOfSupervisor) {
				groups.AddRange(await _coadaptService.Group.GroupListByStudy(study.Id));
			}
			return Ok(groups);
		}

		/// <summary>
		/// Retrieves all groups of specific study
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve groups
		/// -- An administrator retrieves all groups
		/// -- A sub-administrator retrieves all groups of own organization
		/// -- A supervisor retrieves only groups of own studies
		/// </remarks>
		[HttpGet("study/{studyId}", Name = "GroupsByStudyId")]
		[ProducesResponseType(typeof(Group), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetGroupsByStudyId(int studyId) {
			var study = await _coadaptService.Study.GetStudyByIdAsync(studyId);
			if (study.IsEmptyObject()) {
				_logger.LogError($"GetGroupsByStudyId: Study with ID {studyId} not found.");
				return NotFound("Study with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.AdministratorRole) {
				return Ok(await _coadaptService.Group.GroupsByStudy(studyId));
			}
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetGroupsByStudyId: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId == organization.Id) {
					return Ok(await _coadaptService.Group.GroupsByStudy(studyId));
				}
				_logger.LogWarn($"GetGroupsByStudyId: Requested groups do not belong to a study of the organization of the currently logged in sub-administrator!");
				return BadRequest("Requested groups do not belong to a study of the organization of the currently logged in sub-administrator");
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (study.SupervisorId == supervisor.Id) {
				return Ok(await _coadaptService.Group.GroupsByStudy(studyId));
			}
			_logger.LogWarn($"GetGroupsByStudyId: Requested groups do not belong to a study of the currently logged in supervisor!");
			return BadRequest("Requested groups do not belong to a study of the currently logged in supervisor");
		}

		/// <summary>
		///	Retrieve the group with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve a group
		/// -- An administrator retrieves any group
		/// -- A sub-administrator retrieves a group if it belongs to a study of own organization
		/// -- A supervisor retrieves a group if it belongs to own studies
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "GroupById")]
		[ProducesResponseType(typeof(Group), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetGroupById(int id) {
			var group = await _coadaptService.Group.GetGroupByIdAsync(id);
			if (group.IsEmptyObject()) {
				_logger.LogError($"GetGroupById: Group with ID {id} not found.");
				return NotFound("Group with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var study = await _coadaptService.Study.GetStudyByIdAsync(group.StudyId);
			if (role == Role.AdministratorRole) {
				group.Study = study;
				return Ok(group);
			}
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetGroupById: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId == organization.Id) {
					group.Study = study;
					return Ok(group);
				}
				_logger.LogWarn($"GetGroupById: Requested group does not belong of to a study of the organization of the currently logged in sub-administrator!");
				return BadRequest("Requested group does not belong of to a study of the organization of the currently logged in sub-administrator");
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (study.SupervisorId == supervisor.Id) {
				group.Study = study;
				return Ok(group);
			}
			_logger.LogWarn($"GetGroupById: Requested group does not belong to a study of the currently logged in supervisor!");
			return BadRequest("Requested group does not belong to a study of the currently logged in supervisor");
		}

		/// <summary>
		///	Retrieve the group with short name and parent study ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve a group
		/// -- An administrator retrieves any group
		/// -- A sub-administrator retrieves a group if it belongs to a study of own organization
		/// -- A supervisor retrieves a group if it belongs to own studies
		/// </remarks>
		/// <param name="shortName"></param>
		/// <param name="studyId"></param>
		[HttpGet("{shortName}/{studyId}", Name = "GroupByShortNameAndStudyId")]
		[ProducesResponseType(typeof(Group), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetGroupByShortNameAndStudyId(string shortName, int studyId) {
			var group = await _coadaptService.Group.GetGroupOfStudyByShortnameAsync(shortName, studyId);
			if (group.IsEmptyObject()) {
				_logger.LogError($"GetGroupByShortNameAndStudyId: Group with short name {shortName} not found in study with ID {studyId}.");
				return NotFound("Group with requested short name and parent study ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var study = await _coadaptService.Study.GetStudyByIdAsync(group.StudyId);
			if (role == Role.AdministratorRole) {
				group.Study = study;
				return Ok(group);
			}
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"GetGroupByShortNameAndStudyId: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId == organization.Id) {
					group.Study = study;
					return Ok(group);
				}
				_logger.LogWarn($"GetGroupByShortNameAndStudyId: Requested group does not belong of to a study of the organization of the currently logged in sub-administrator!");
				return BadRequest("Requested group does not belong of to a study of the organization of the currently logged in sub-administrator");
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (study.SupervisorId == supervisor.Id) {
				group.Study = study;
				return Ok(group);
			}
			_logger.LogWarn($"GetGroupByShortNameAndStudyId: Requested group does not belong to a study of the currently logged in supervisor!");
			return BadRequest("Requested group does not belong to a study of the currently logged in supervisor");
		}

		/// <summary>
		///	Retrieve the group with given path of short names [organization short name].[study short name].[group short name]
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can retrieve a group
		/// -- An administrator retrieves any group
		/// -- A sub-administrator retrieves a group if it belongs to a study of own organization
		/// -- A supervisor retrieves a group if it belongs to own studies
		/// </remarks>
		/// <param name="names"></param>
		[HttpGet("names/{names}", Name = "GroupByShortNamesPath")]
		[ProducesResponseType(typeof(Group), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetGroupByShortNamesPath(string names) {
			var nameComponents = names.Split(".");
			if (nameComponents.Length != 3) {
				_logger.LogWarn($"GetGroupByShortNamesPath: Expected three short names separated py periods: [organization short name].[study short name].[group short name]");
				return BadRequest("Expected three short names separated py periods: [organization short name].[study short name].[group short name]");
			}
			var organization = await _coadaptService.Organization.GetOrganizationByShortnameAsync(nameComponents[0]);
			if (organization.IsEmptyObject()) {
				_logger.LogWarn($"GetGroupByShortNamesPath: Organization with short name {nameComponents[0]} does not exist!");
				return BadRequest("Organization with requested short name does not exist");
			}
			var study = await _coadaptService.Study.GetStudyOfOrganizationByShortnameAsync(nameComponents[1], organization.Id);
			if (study.IsEmptyObject()) {
				_logger.LogWarn($"GetGroupByShortNamesPath: Study with short name {nameComponents[1]} does not exist in organization with short name {nameComponents[0]}!");
				return BadRequest("Study with requested short name does not exist in specified organization");
			}
			var group = await _coadaptService.Group.GetGroupOfStudyByShortnameAsync(nameComponents[2], study.Id);
			if (group.IsEmptyObject()) {
				_logger.LogError($"GetGroupByShortNamesPath: Group with short name {nameComponents[2]} not found in parent study.");
				return NotFound("Group with requested short name and parent study does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.AdministratorRole) {
				group.Study = study;
				return Ok(group);
			}
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organizationOfSubAdmin = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organizationOfSubAdmin.IsEmptyObject()) {
					_logger.LogWarn($"GetGroupByShortNamesPath: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId == organizationOfSubAdmin.Id) {
					group.Study = study;
					return Ok(group);
				}
				_logger.LogWarn($"GetGroupByShortNamesPath: Requested group does not belong of to a study of the organization of the currently logged in sub-administrator!");
				return BadRequest("Requested group does not belong of to a study of the organization of the currently logged in sub-administrator");
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (study.SupervisorId == supervisor.Id) {
				group.Study = study;
				return Ok(group);
			}
			_logger.LogWarn($"GetGroupByShortNamesPath: Requested group does not belong to a study of the currently logged in supervisor!");
			return BadRequest("Requested group does not belong to a study of the currently logged in supervisor");
		}

		/// <summary>
		/// Create a new group
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can create a group
		/// -- An administrator creates any group
		/// -- A sub-administrator creates a group belonging to a study of own organization
		/// -- A supervisor creates a group belonging to own studies
		/// - Group name cannot be empty
		/// - Group short name cannot be empty and must be unique amongst groups of the same study
		/// - The group is assigned to a valid study at creation
		/// - Groups are created without participants; participants are assigned to groups at participant creation or update
		/// </remarks>
		/// <param name="groupRequest"></param>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateGroup([FromBody]GroupRequest groupRequest) {
			if (groupRequest == null) {
				_logger.LogError("CreateGroup: GroupRequest object sent from client is null.");
				return BadRequest("GroupRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateGroup: Invalid GroupRequest object sent from client.");
				return BadRequest("Invalid GroupRequest object");
			}
			if (groupRequest.Name == "") {
				_logger.LogError("CreateGroup: Group name cannot be empty.");
				return BadRequest("Group name cannot be empty");
			}
			if (groupRequest.Shortname == "") {
				_logger.LogError("CreateGroup: Group short name cannot be empty.");
				return BadRequest("Group short name cannot be empty");
			}
			var study = await _coadaptService.Study.GetStudyByIdAsync(groupRequest.StudyId);
			if (study.IsEmptyObject()) {
				_logger.LogError($"CreateGroup: Study with ID {groupRequest.StudyId} not found.");
				return BadRequest("Study with requested ID does not exist");
			}
			var groupOfSameShortname = await _coadaptService.Group.GetGroupOfStudyByShortnameAsync(groupRequest.Shortname, groupRequest.StudyId);
			if (!groupOfSameShortname.IsEmptyObject()) {
				_logger.LogError($"CreateGroup: Group with same short name already exists in study!");
				return BadRequest("Group with same short name already exists in study");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"CreateGroup: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId != organization.Id) {
					_logger.LogWarn($"CreateGroup: Requested study does not belong to the organization of the currently logged in sub-administrator!");
					return BadRequest("Requested study does not belong to the organization of the currently logged in sub-administrator");
				}
			} else if (role == Role.SupervisorRole) {
				var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (study.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"CreateGroup: Requested study does not belong to the currently logged in supervisor!");
					return BadRequest("Requested study does not belong to the currently logged in supervisor");
				}
			}
			var group = new Group { Name = groupRequest.Name, Shortname = groupRequest.Shortname, StudyId = groupRequest.StudyId };
			_coadaptService.Group.CreateGroup(group);
			await _coadaptService.SaveAsync();
			group.Study = study;
			return CreatedAtRoute("SiteById", new { id = group.Id }, group);
		}

		/// <summary>
		/// Update an existing group with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can update a group
		/// -- An administrator updates any group
		/// -- A sub-administrator updates a group belonging to a study of own organization and cannot reassign it to a study of a different organization
		/// -- A supervisor creates a group belonging to own studies and cannot reassign it to a study other than those
		/// - Request an empty name or short name, or zero study ID in order not to change that item
		/// - Group short name must be unique amongst groups of study
		/// - The requested study ID (if not zero) must already be created
		/// - A group with participants cannot be moved to a different study
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="groupRequest"></param>
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateGroup(int id, [FromBody]GroupRequest groupRequest) {
			if (groupRequest == null) {
				_logger.LogError("UpdateGroup: GroupRequest object sent from client is null.");
				return BadRequest("GroupRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateGroup: Invalid GroupRequest object sent from client.");
				return BadRequest("Invalid GroupRequest object");
			}
			var dbGroup = await _coadaptService.Group.GetGroupByIdAsync(id);
			if (dbGroup.IsEmptyObject()) {
				_logger.LogError($"UpdateGroup: Group with ID {id} not found.");
				return NotFound("Group with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			SubAdministrator subAdministrator = new SubAdministrator();
			Organization organization = new Organization();
			Supervisor supervisor = new Supervisor();
			var originalStudy = await _coadaptService.Study.GetStudyByIdAsync(dbGroup.StudyId);
			if (role == Role.SubAdministratorRole) {
				subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"UpdateGroup: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (originalStudy.OrganizationId != organization.Id) {
					_logger.LogWarn($"UpdateGroup: Study of updated group does not belong to the organization of the currently logged in sub-administrator!");
					return BadRequest("Study of updated group does not belong to the organization of the currently logged in sub-administrator");
				}
			} else if (role == Role.SupervisorRole) {
				supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (originalStudy.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"UpdateGroup: Study of updated group does not belong to the currently logged in supervisor!");
					return BadRequest("Study of updated group does not belong to the currently logged in supervisor");
				}
			}
			if (groupRequest.StudyId == 0) {
				groupRequest.StudyId = dbGroup.StudyId;
			}
			if (groupRequest.StudyId != dbGroup.StudyId) {
				var study = await _coadaptService.Study.GetStudyByIdAsync(groupRequest.StudyId);
				if (study.IsEmptyObject()) {
					_logger.LogError($"UpdateGroup: Study with ID {groupRequest.StudyId} not found.");
					return BadRequest("Study with requested ID does not exist");
				}
				if (role == Role.SubAdministratorRole) {
					if (study.OrganizationId != organization.Id) {
						_logger.LogWarn($"UpdateGroup: Requested study does not belong to the organization of the currently logged in sub-administrator!");
						return BadRequest("Requested study does not belong to the organization of the currently logged in sub-administrator");
					}
				} else if (role == Role.SupervisorRole) {
					if (study.SupervisorId != supervisor.Id) {
						_logger.LogWarn($"UpdateGroup: Requested study does not belong to the currently logged in supervisor!");
						return BadRequest("Requested study does not belong to the currently logged in supervisor");
					}
				}
			}
			if (groupRequest.Shortname == "") {
				groupRequest.Shortname = dbGroup.Shortname;
			}
			if (groupRequest.Shortname != dbGroup.Shortname) {
				var groupOfSameShortname = await _coadaptService.Group.GetGroupOfStudyByShortnameAsync(groupRequest.Shortname, groupRequest.StudyId);
				if (!groupOfSameShortname.IsEmptyObject()) {
					_logger.LogError($"UpdateGroup: Group with same short name already exists in study!");
					return BadRequest("Group with same short name already exists in study");
				}
			}
			if (groupRequest.Name == "") {
				groupRequest.Name = dbGroup.Name;
			}
			var group = new Group { Name = groupRequest.Name, Shortname = groupRequest.Shortname, StudyId = groupRequest.StudyId };
			_coadaptService.Group.UpdateGroup(dbGroup, group);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Delete the group with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators and supervisors can delete a group
		/// -- An administrator deletes any group
		/// -- A sub-administrator deletes a group if it belongs to a study of own organization
		/// -- A supervisor deletes a group if it belongs to own studies
		/// - Cannot delete group that has participants; un-assign or delete the participants first
		/// </remarks>
		/// <param name="id"></param>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteGroup(int id) {
			var group = await _coadaptService.Group.GetGroupByIdAsync(id);
			if (group.IsEmptyObject()) {
				_logger.LogError($"DeleteGroup: Group with ID {id} not found.");
				return NotFound("Group with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			var study = await _coadaptService.Study.GetStudyByIdAsync(group.StudyId);
			if (role == Role.SubAdministratorRole) {
				var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
				if (organization.IsEmptyObject()) {
					_logger.LogWarn($"DeleteGroup: Currently logged in sub-administrator is not assigned to any organization!");
					return BadRequest("Currently logged in sub-administrator is not assigned to any organization");
				}
				if (study.OrganizationId != organization.Id) {
					_logger.LogWarn($"DeleteGroup: Study of group to delete does not belong to the organization of the currently logged in sub-administrator!");
					return BadRequest("Study of group to delete does not belong to the organization of the currently logged in sub-administrator");
				}
			} else if (role == Role.SupervisorRole) {
				var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (study.SupervisorId != supervisor.Id) {
					_logger.LogWarn($"DeleteGroup: Study of group to delete does not belong to the currently logged in supervisor!");
					return BadRequest("Study of group to delete does not belong to the currently logged in supervisor");
				}
			}
			if (((ICollection<StudyParticipant>)await _coadaptService.StudyParticipant.StudyParticipantsByGroup(id, false)).Count > 0) {
				_logger.LogError($"DeleteGroup: Cannot delete group with participants.");
				return BadRequest("Cannot delete group with participants");
			}
			_coadaptService.Group.DeleteGroup(group);
			await _coadaptService.SaveAsync();
			return NoContent();
		}

	}

}
