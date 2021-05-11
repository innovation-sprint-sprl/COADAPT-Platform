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

	[Route("1.0/subAdmins")]
	[ApiController]
	[Authorize(Policy = "SubAdministrator")]
	public class SubAdministratorController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public SubAdministratorController(ILoggerManager logger, IRepositoryWrapper repository,
			UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
			_logger = logger;
			_coadaptService = repository;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		/// <summary>
		/// Retrieve all sub-administrators
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can retrieve all sub-administrators
		/// </remarks>
		[Authorize(Policy = "Administrator")]
		[HttpGet]
		[ProducesResponseType(typeof(SubAdministratorListResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllSubAdministrators() {
			return Ok(await _coadaptService.SubAdministrator.GetAllSubAdministratorsAsync());
		}

		/// <summary>
		///	Retrieve the sub-administrator with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators and sub-administrators can retrieve the sub-administrator with given ID
		/// -- Administrators can retrieve any sub-administrator
		/// -- Sub-administrators can only retrieve self
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "SubAdministratorById")]
		[ProducesResponseType(typeof(SubAdministrator), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetSubAdministratorById(int id) {
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
				var loggedSubAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				if (loggedSubAdministrator.Id != id) {
					_logger.LogWarn($"SubAdministratorById: Currently logged in sub-administrator is not requesting to retrieve self!");
					return BadRequest("Currently logged in sub-administrator is not requesting to retrieve self");
				}
			}
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(id);
			if (subAdministrator.IsEmptyObject()) {
				_logger.LogWarn($"GetSubAdministratorById: Sub-administrator with ID {id} not found!");
				return NotFound("Sub-administrator with requested ID does not exist");
			}
			subAdministrator.User = await _userManager.FindByIdAsync(subAdministrator.UserId);
			return Ok(subAdministrator);
		}

		/// <summary>
		///	Retrieve the currently logged-in sub-administrator
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only a sub-administrator can retrieve self
		/// </remarks>
		[HttpGet("self")]
		[ProducesResponseType(typeof(SubAdministrator), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetLoggedinSubAdministrator() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			if (subAdministrator.IsEmptyObject()) {
				_logger.LogWarn($"GetLoggedinSubAdministrator: Currently logged in user is not a sub-administrator!");
				return BadRequest("Currently logged in user is not a sub-administrator");
			}
			subAdministrator.User = await _userManager.FindByIdAsync(subAdministrator.UserId);
			return Ok(subAdministrator);
		}

		/// <summary>
		/// Create a new sub-administrator
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can create a new sub-administrator
		/// - Username must be unique
		/// - Newly created sub-administrators are not assigned to organizations; a sub-administrator is assigned to an organization at organization creation or update
		/// </remarks>
		/// <param name="userRequest"></param>
		[Authorize(Policy = "Administrator")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateSubAdministrator([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("CreateSubAdministrator: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateSubAdministrator: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			var user = await _userManager.FindByNameAsync(userRequest.UserName);
			if (user != null) {
				_logger.LogError("CreateSubAdministrator: username already exists.");
				return BadRequest("Username already exists");
			}
			user = new IdentityUser { UserName = userRequest.UserName };
			var passwordValidator = new PasswordValidator<IdentityUser>();
			if (!(await passwordValidator.ValidateAsync(_userManager, null, userRequest.Password)).Succeeded) {
				_logger.LogError("CreateSubAdministrator: Provided password is not strong enough.");
				return BadRequest("Provided password is not strong enough");
			}
			await _userManager.CreateAsync(user, userRequest.Password);
			var subAdministrator = new SubAdministrator { UserId = user.Id };
			_coadaptService.SubAdministrator.CreateSubAdministrator(subAdministrator);
			await _coadaptService.SaveAsync();
			if (!await _roleManager.RoleExistsAsync(Role.SubAdministratorRole)) {
				await _roleManager.CreateAsync(new IdentityRole(Role.SubAdministratorRole));
			}
			await _userManager.AddToRoleAsync(user, Role.SubAdministratorRole);
			return CreatedAtRoute("SubAdministratorById", new { id = subAdministrator.Id }, subAdministrator);
		}

		/// <summary>
		/// Delete the sub-administrator with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can delete the sub-administrator with given ID
		/// - Cannot delete a sub-administrator associated with an organization; delete organization first
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "Administrator")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteSubAdministrator(int id) {
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(id);
			if (subAdministrator.IsEmptyObject()) {
				_logger.LogDebug($"DeleteSubAdministrator: Sub-administrator with ID {id} not found.");
				return NotFound("Sub-administrator with requested ID does not exist");
			}
			var organization = await _coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(id);
			if (!organization.IsEmptyObject()) {
				_logger.LogError($"DeleteSubAdministrator: Cannot delete sub-administrator associated with organization.");
				return BadRequest("Cannot delete sub-administrator associated with organization");
			}
			var user = await _userManager.FindByIdAsync(subAdministrator.UserId);
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
			_coadaptService.SubAdministrator.DeleteSubAdministrator(subAdministrator);
			result = await _userManager.DeleteAsync(user);
			if (result != IdentityResult.Success) {
				return StatusCode(StatusCodes.Status500InternalServerError, "Cannot delete user");
			}
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Update an existing sub-administrator with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators and sub-administrators can update an existing sub-administrator with given ID
		/// -- Administrators can update any sub-administrator
		/// -- Sub-administrators can only update self
		/// - Username must be unique
		/// - Request an empty user name or password in order not to change them
		/// - Sub-administrators are not assigned to organizations at sub-administrator update; a sub-administrator is assigned to an organization at organization creation or update
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="userRequest"></param>
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateSubAdministrator(int id, [FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("UpdateSubAdministrator: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateSubAdministrator: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
				var loggedSubAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				if (loggedSubAdministrator.Id != id) {
					_logger.LogWarn($"UpdateSubAdministrator: Currently logged in sub-administrator is not requesting to update self!");
					return BadRequest("Currently logged in sub-administrator is not requesting to update self");
				}
			}
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByIdAsync(id);
			if (subAdministrator.IsEmptyObject()) {
				_logger.LogError("UpdateSubAdministrator: Sub-administrator with requested ID does not exist.");
				return NotFound("Sub-administrator with requested ID does not exist");
			}
			if (userRequest.UserName == "" && userRequest.Password == "") {
				return NoContent();
			}
			var user = await _userManager.FindByIdAsync(subAdministrator.UserId);
			if (userRequest.Password != "") {
				var passwordValidator = new PasswordValidator<IdentityUser>();
				if (!(await passwordValidator.ValidateAsync(_userManager, null, userRequest.Password)).Succeeded) {
					_logger.LogError("UpdateSubAdministrator: Provided password is not strong enough.");
					return BadRequest("Provided password is not strong enough");
				}
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				await _userManager.ResetPasswordAsync(user, token, userRequest.Password);
			}
			if (userRequest.UserName != "") {
				var dbUser = await _userManager.FindByNameAsync(userRequest.UserName);
				if (dbUser != null && user.Id != dbUser.Id) {
					_logger.LogError("UpdateSubAdministrator: Username already exists.");
					return BadRequest("Username already exists");
				}
				await _userManager.SetUserNameAsync(user, userRequest.UserName);
			}
			return NoContent();
		}

		/// <summary>
		/// Update the currently logged in sub-administrator
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only sub-administrators can update self
		/// - Username must be unique
		/// - Request an empty user name or password in order not to change them
		/// - Sub-administrators are not assigned to organizations at sub-administrator update; a sub-administrator is assigned to an organization at organization creation or update
		/// </remarks>
		/// <param name="userRequest"></param>
		[HttpPut("self")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateLoggedinSubAdministrator([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("UpdateLoggedinSubAdministrator: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateLoggedinSubAdministrator: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var subAdministrator = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
			if (subAdministrator.IsEmptyObject()) {
				_logger.LogWarn($"UpdateLoggedinSubAdministrator: Currently logged in user is not a sub-administrator!");
				return BadRequest("Currently logged in user is not a sub-administrator");
			}
			if (userRequest.UserName == "" && userRequest.Password == "") {
				return NoContent();
			}
			var user = await _userManager.FindByIdAsync(subAdministrator.UserId);
			if (userRequest.UserName != "") {
				var dbUser = await _userManager.FindByNameAsync(userRequest.UserName);
				if (dbUser != null && user.Id != dbUser.Id) {
					_logger.LogError("UpdateLoggedinSubAdministrator: Username already exists.");
					return BadRequest("Username already exists");
				}
				await _userManager.SetUserNameAsync(user, userRequest.UserName);
			}
			if (userRequest.Password != "") {
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				await _userManager.ResetPasswordAsync(user, token, userRequest.Password);
			}
			return NoContent();
		}

	}
}
