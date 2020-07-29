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

	[Route("1.0/therapists")]
	[ApiController]
	[Authorize(Policy = "Therapist")]
	public class TherapistController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public TherapistController(ILoggerManager logger, IRepositoryWrapper repository,
			UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
			_logger = logger;
			_coadaptService = repository;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		/// <summary>
		/// Retrieve all therapists
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can retrieve all therapists
		/// </remarks>
		[Authorize(Policy = "Supervisor")]
		[HttpGet]
		[ProducesResponseType(typeof(Therapist), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllTherapists() {
			return Ok(await _coadaptService.Therapist.GetAllTherapistsAsync());
		}

		/// <summary>
		///	Retrieve the therapist with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators, supervisors or therapists can retrieve the therapist with given ID
		/// -- Administrators, sub-administrators or supervisors can retrieve any therapist
		/// -- Therapists can only retireve self
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "TherapistById")]
		[ProducesResponseType(typeof(Supervisor), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetTherapistById(int id) {
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.TherapistRole) {
				string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
				var loggedTherapist = await _coadaptService.Therapist.GetTherapistByUserIdAsync(userId);
				if (loggedTherapist.Id != id) {
					_logger.LogWarn($"GetTherapistById: Currently logged in therapist is not retrieving self!");
					return BadRequest("Currently logged in therapist is not retrieving self");
				}
			}
			var therapist = await _coadaptService.Therapist.GetTherapistByIdAsync(id);
			if (therapist.IsEmptyObject()) {
				_logger.LogWarn($"GetTherapistById: Therapist with ID {id} not found!");
				return NotFound("Therapist with requested ID does not exist");
			}
			therapist.User = await _userManager.FindByIdAsync(therapist.UserId);
			therapist.Participants = (ICollection<Participant>)await _coadaptService.Participant.GetParticipantsByTherapistIdAsync(id);
			return Ok(therapist);
		}

		/// <summary>
		/// Create a new therapist
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can create a new therapist
		/// - Username must be unique
		/// - Newly created therapists are not assigned any participant; a participant is assigned to a therapist at participant creation or update
		/// </remarks>
		/// <param name="userRequest"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateTherapist([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("CreateTherapist: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateTherapist: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			var user = await _userManager.FindByNameAsync(userRequest.UserName);
			if (user != null) {
				_logger.LogError("CreateTherapist: username already exists.");
				return BadRequest("Username already exists");
			}
			user = new IdentityUser { UserName = userRequest.UserName };
			var passwordValidator = new PasswordValidator<IdentityUser>();
			if (!(await passwordValidator.ValidateAsync(_userManager, null, userRequest.Password)).Succeeded) {
				_logger.LogError("CreateTherapist: Provided password is not strong enough.");
				return BadRequest("Provided password is not strong enough");
			}
			await _userManager.CreateAsync(user, userRequest.Password);
			var therapist = new Therapist { UserId = user.Id };
			_coadaptService.Therapist.CreateTherapist(therapist);
			await _coadaptService.SaveAsync();
			if (!await _roleManager.RoleExistsAsync(Role.TherapistRole)) {
				await _roleManager.CreateAsync(new IdentityRole(Role.TherapistRole));
			}
			await _userManager.AddToRoleAsync(user, Role.TherapistRole);
			return CreatedAtRoute("TherapistById", new { id = therapist.Id }, therapist);
		}

		/// <summary>
		/// Delete the therapist with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators or supervisors can delete a therapist
		/// - Cannot delete a therapist associated with participants; assign these participants to different therapists or delete them first
		/// </remarks>
		/// <param name="id"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteTherapist(int id) {
			var therapist = await _coadaptService.Therapist.GetTherapistByIdAsync(id);
			if (therapist.IsEmptyObject()) {
				_logger.LogDebug($"DeleteTherapist: Therapist with ID {id} not found.");
				return NotFound("Therapist with requested ID does not exist");
			}
			if (await _coadaptService.Participant.CountParticipantsByTherapistIdAsync(id) > 0) {
				_logger.LogError($"DeleteTherapist: Cannot delete therapist associated with participants.");
				return BadRequest("Cannot delete therapist associated with participants");
			}
			var user = await _userManager.FindByIdAsync(therapist.UserId);
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
			_coadaptService.Therapist.DeleteTherapist(therapist);
			result = await _userManager.DeleteAsync(user);
			if (result != IdentityResult.Success) {
				return StatusCode(StatusCodes.Status500InternalServerError, "Cannot delete user");
			}
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Update an existing therapist with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators, sub-administrators, supervisors or therapists can update a therapist
		/// -- Administrators, sub-administrators or supervisors can update any therapist
		/// -- Therapists can only update self
		/// - Username must be unique
		/// - Request an empty user name or password in order not to change them
		/// - Therapists are not assigned any participant at therapist update; a participant is assigned to a therapist at participant creation or update
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="userRequest"></param>
		[Authorize(Policy = "Supervisor")]
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateTherapist(int id, [FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("UpdateTherapist: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateTherapist: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			string role = HttpContext.User.Claims.Single(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
			if (role == Role.TherapistRole) {
				string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
				var loggedTherapist = await _coadaptService.Therapist.GetTherapistByUserIdAsync(userId);
				if (loggedTherapist.Id != id) {
					_logger.LogWarn($"UpdateTherapist: Currently logged in therapist is not requesting to update self!");
					return BadRequest("Currently logged in therapist is not requesting to update self");
				}
			}
			var therapist = await _coadaptService.Therapist.GetTherapistByIdAsync(id);
			if (therapist.IsEmptyObject()) {
				_logger.LogError("UpdateTherapist: Therapist with requested ID does not exist.");
				return NotFound("Therapist with requested ID does not exist");
			}
			if (userRequest.UserName == "" && userRequest.Password == "") {
				return NoContent();
			}
			var user = await _userManager.FindByIdAsync(therapist.UserId);
			if (userRequest.Password != "") {
				var passwordValidator = new PasswordValidator<IdentityUser>();
				if (!(await passwordValidator.ValidateAsync(_userManager, null, userRequest.Password)).Succeeded) {
					_logger.LogError("UpdateTherapist: Provided password is not strong enough.");
					return BadRequest("Provided password is not strong enough");
				}
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				await _userManager.ResetPasswordAsync(user, token, userRequest.Password);
			}
			if (userRequest.UserName != "") {
				var dbUser = await _userManager.FindByNameAsync(userRequest.UserName);
				if (dbUser != null && user.Id != dbUser.Id) {
					_logger.LogError("UpdateTherapist: username already exists.");
					return BadRequest("Username already exists");
				}
				await _userManager.SetUserNameAsync(user, userRequest.UserName);
			}
			return NoContent();
		}

		/// <summary>
		///	Retrieve the currently logged-in therapist
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only therapists can retrieve self
		/// </remarks>
		[HttpGet("self")]
		[ProducesResponseType(typeof(Therapist), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetLoggedinTherapist() {
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var therapist = await _coadaptService.Therapist.GetTherapistByUserIdAsync(userId);
			if (therapist.IsEmptyObject()) {
				_logger.LogWarn($"GetLoggedinTherapist: Currently logged in user is not a therapist!");
				return BadRequest("Currently logged in user is not a therapist");
			}
			therapist.User = await _userManager.FindByIdAsync(therapist.UserId);
			return Ok(therapist);
		}

		/// <summary>
		/// Update the currently logged in therapist
		/// </summary>
		/// <remarks>
		/// - Only therapists can update self
		/// - Username must be unique
		/// - Request an empty user name or password in order not to change them
		/// - Therapists are not assigned any participant at therapist update; a participant is assigned to a therapist at participant creation or update
		/// </remarks>
		/// <param name="userRequest"></param>
		[HttpPut("self")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateLoggedinTherapist([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("UpdateLoggedinTherapist: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateLoggedinTherapist: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			var therapist = await _coadaptService.Therapist.GetTherapistByUserIdAsync(userId);
			if (therapist.IsEmptyObject()) {
				_logger.LogWarn($"UpdateLoggedinTherapist: Currently logged in user is not a therapist!");
				return BadRequest("Currently logged in user is not a therapist");
			}
			if (userRequest.UserName == "" && userRequest.Password == "") {
				return NoContent();
			}
			var user = await _userManager.FindByIdAsync(therapist.UserId);
			if (userRequest.UserName != "") {
				var dbUser = await _userManager.FindByNameAsync(userRequest.UserName);
				if (dbUser != null && user.Id != dbUser.Id) {
					_logger.LogError("UpdateLoggedinTherapist: Username already exists.");
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
