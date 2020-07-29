using Constants;
using Contracts.Logger;
using Contracts.Repository;
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

namespace UserManagement.WebAPI.Controllers {

	[Route("1.0/supervisors")]
	[ApiController]
	[Authorize(Policy = "Supervisor")]
	public class SupervisorController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public SupervisorController(ILoggerManager logger, IRepositoryWrapper repository,
			UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
			_logger = logger;
			_coadaptService = repository;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		/// <summary>
		/// Retrieve all supervisors
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators or sub-administrators can retrieve all supervisors
		/// </remarks>
		[Authorize(Policy = "SubAdministrator")]
		[HttpGet]
		[ProducesResponseType(typeof(Supervisor), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllSupervisors() {
			return Ok(await _coadaptService.Supervisor.GetAllSupervisorsAsync());
		}

		/// <summary>
		///	Retrieve the supervisor with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can retrieve the supervisor with given ID
		/// -- Administrators or sub-administrators can retrieve any supervisor
		/// -- Supervisors can only retrieve self
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "SupervisorById")]
		[ProducesResponseType(typeof(Supervisor), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetSupervisorById(int id) {
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SupervisorRole) {
				string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
				var loggedSupervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (loggedSupervisor.Id != id) {
					_logger.LogWarn($"SupervisorById: Currently logged in supervisor is not retrieving self!");
					return BadRequest("Currently logged in supervisor is not retrieving self");
				}
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByIdAsync(id);
			if (supervisor.IsEmptyObject()) {
				_logger.LogWarn($"GetSupervisorById: Supervisor with ID {id} not found!");
				return NotFound("Supervisor with requested ID does not exist");
			}
			supervisor.User = await _userManager.FindByIdAsync(supervisor.UserId);
			supervisor.Studies = (ICollection<Study>)await _coadaptService.Study.GetStudiesBySupervisorIdAsync(id);
			return Ok(supervisor);
		}

		/// <summary>
		///	Retrieve the currently logged-in supervisor
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only supervisors can retrieve self
		/// </remarks>
		[HttpGet("self")]
		[ProducesResponseType(typeof(Supervisor), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetLoggedinSupervisor() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (supervisor.IsEmptyObject()) {
				_logger.LogWarn($"GetLoggedinSupervisor: Currently logged in user is not a supervisor!");
				return BadRequest("Currently logged in user is not a supervisor");
			}
			supervisor.User = await _userManager.FindByIdAsync(supervisor.UserId);
			return Ok(supervisor);
		}

		/// <summary>
		/// Create a new supervisor
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators or sub-administrators can create a new supervisor
		/// - Username must be unique
		/// - Newly created supervisors are not assigned to any study; a supervisor is assigned to a study at study creation
		/// </remarks>
		/// <param name="userRequest"></param>
		[Authorize(Policy = "SubAdministrator")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateSupervisor([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("CreateSupervisor: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateSupervisor: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			var user = await _userManager.FindByNameAsync(userRequest.UserName);
			if (user != null) {
				_logger.LogError("CreateSupervisor: username already exists.");
				return BadRequest("Username already exists");
			}
			user = new IdentityUser { UserName = userRequest.UserName };
			var passwordValidator = new PasswordValidator<IdentityUser>();
			if (!(await passwordValidator.ValidateAsync(_userManager, null, userRequest.Password)).Succeeded) {
				_logger.LogError("CreateSupervisor: Provided password is not strong enough.");
				return BadRequest("Provided password is not strong enough");
			}
			await _userManager.CreateAsync(user, userRequest.Password);
			var supervisor = new Supervisor { UserId = user.Id };
			_coadaptService.Supervisor.CreateSupervisor(supervisor);
			await _coadaptService.SaveAsync();
			if (!await _roleManager.RoleExistsAsync(Role.SupervisorRole)) {
				await _roleManager.CreateAsync(new IdentityRole(Role.SupervisorRole));
			}
			await _userManager.AddToRoleAsync(user, Role.SupervisorRole);
			return CreatedAtRoute("SupervisorById", new { id = supervisor.Id }, supervisor);
		}

		/// <summary>
		/// Delete the supervisor with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators or sub-administrators can delete a supervisor
		/// - Cannot delete supervisor associated with studies; assign to these studies a different supervisor or delete them first
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "SubAdministrator")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteSupervisor(int id) {
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByIdAsync(id);
			if (supervisor.IsEmptyObject()) {
				_logger.LogDebug($"DeleteSupervisor: Supervisor with ID {id} not found.");
				return NotFound("Supervisor with requested ID does not exist");
			}
			if (await _coadaptService.Study.CountStudiesBySupervisorIdAsync(id) > 0) {
				_logger.LogError($"DeleteSupervisor: Cannot delete supervisor associated with studies.");
				return BadRequest("Cannot delete supervisor associated with studies");
			}
			var user = await _userManager.FindByIdAsync(supervisor.UserId);
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
			_coadaptService.Supervisor.DeleteSupervisor(supervisor);
			result = await _userManager.DeleteAsync(user);
			if (result != IdentityResult.Success) {
				return StatusCode(StatusCodes.Status500InternalServerError, "Cannot delete user");
			}
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Update an existing supervisor with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can update a supervisor
		/// -- Administrators or sub-administrators can update any supervisor
		/// -- Supervisors can only update self
		/// - Username must be unique
		/// - Request an empty user name or password in order not to change them
		/// - Supervisors are not assigned to studies via supervisor update; study create and update endpoints are used for that
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
		public async Task<IActionResult> UpdateSupervisor(int id, [FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("UpdateSupervisor: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateSupervisor: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.SubAdministratorRole) {
				string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
				var loggedSupervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
				if (loggedSupervisor.Id != id) {
					_logger.LogWarn($"UpdateSupervisor: Currently logged in supervisor is not requesting to update self!");
					return BadRequest("Currently logged in supervisor is not requesting to update self");
				}
			}
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByIdAsync(id);
			if (supervisor.IsEmptyObject()) {
				_logger.LogError("UpdateSupervisor: Supervisor with requested ID does not exist.");
				return NotFound("Supervisor with requested ID does not exist");
			}
			if (userRequest.UserName == "" && userRequest.Password == "") {
				return NoContent();
			}
			var user = await _userManager.FindByIdAsync(supervisor.UserId);
			if (userRequest.Password != "") {
				var passwordValidator = new PasswordValidator<IdentityUser>();
				if (!(await passwordValidator.ValidateAsync(_userManager, null, userRequest.Password)).Succeeded) {
					_logger.LogError("UpdateSupervisor: Provided password is not strong enough.");
					return BadRequest("Provided password is not strong enough");
				}
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				await _userManager.ResetPasswordAsync(user, token, userRequest.Password);
			}
			if (userRequest.UserName != "") {
				var dbUser = await _userManager.FindByNameAsync(userRequest.UserName);
				if (dbUser != null && user.Id != dbUser.Id) {
					_logger.LogError("UpdateSupervisor: Username already exists.");
					return BadRequest("Username already exists");
				}
				await _userManager.SetUserNameAsync(user, userRequest.UserName);
			}
			return NoContent();
		}

		/// <summary>
		/// Update the currently logged in supervisor
		/// </summary>
		/// <remarks>
		/// - Only supervisors can do this
		/// - Username must be unique
		/// - Request an empty user name or password in order not to change them
		/// - Supervisors are not assigned to studies via supervisor update; study create and update endpoints are used for that
		/// </remarks>
		/// <param name="userRequest"></param>
		[HttpPut("self")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateLoggedinSupervisor([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("UpdateLoggedinSupervisor: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateLoggedinSupervisor: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var supervisor = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(userId);
			if (supervisor.IsEmptyObject()) {
				_logger.LogWarn($"UpdateLoggedinSupervisor: Currently logged in user is not a supervisor!");
				return BadRequest("Currently logged in user is not a supervisor");
			}
			if (userRequest.UserName == "" && userRequest.Password == "") {
				return NoContent();
			}
			var user = await _userManager.FindByIdAsync(supervisor.UserId);
			if (userRequest.UserName != "") {
				var dbUser = await _userManager.FindByNameAsync(userRequest.UserName);
				if (dbUser != null && user.Id != dbUser.Id) {
					_logger.LogError("UpdateLoggedinSupervisor: Username already exists.");
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
