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
using Entities.Config;
using Microsoft.Extensions.Options;
using ApiModels;

namespace UserManagement.WebAPI.Controllers {

	[Route("1.0/admin")]
	[ApiController]
	[Authorize(Policy = "Administrator")]
	public class AdministratorController : ControllerBase {

		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IOptions<AppSettings> _appSettings;

		public AdministratorController(ILoggerManager logger,
			IRepositoryWrapper repository,
			UserManager<IdentityUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IOptions<AppSettings> appSettings) {
			_logger = logger;
			_coadaptService = repository;
			_userManager = userManager;
			_roleManager = roleManager;
			_appSettings = appSettings;
		}

		/// <summary>
		/// Retrieve all administrators
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can retrieve all administrators
		/// </remarks>
		[HttpGet]
		[ProducesResponseType(typeof(AdministratorResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAllAdministrators() {
			return Ok(await _coadaptService.Administrator.GetAllAdministratorsAsync());
		}

		/// <summary>
		///	Retrieve the administrator with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can retrieve an administrator
		/// </remarks>
		/// <param name="id"></param>
		[HttpGet("{id}", Name = "AdministratorById")]
		[ProducesResponseType(typeof(Administrator), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetAdministratorById(int id) {
			var administrator = await _coadaptService.Administrator.GetAdministratorByIdAsync(id);
			if (administrator.IsEmptyObject()) {
				_logger.LogWarn($"GetAdministratorById: Administrator with ID {id} not found!");
				return NotFound("Administrator with requested ID does not exist");
			}
			administrator.User = await _userManager.FindByIdAsync(administrator.UserId);
			return Ok(administrator);
		}

		/// <summary>
		/// Create a new administrator
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can create an administrator
		/// - Username must be unique
		/// </remarks>
		/// <param name="userRequest"></param>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateAdministrator([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("CreateAdministrator: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("CreateAdministrator: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			var user = await _userManager.FindByNameAsync(userRequest.UserName);
			if (user != null) {
				_logger.LogError("CreateAdministrator: username already exists.");
				return BadRequest("Username already exists");
			}
			user = new IdentityUser { UserName = userRequest.UserName };
			var passwordValidator = new PasswordValidator<IdentityUser>();
			if (!(await passwordValidator.ValidateAsync(_userManager, null, userRequest.Password)).Succeeded) {
				_logger.LogError("CreateAdministrator: Provided password is not strong enough.");
				return BadRequest("Provided password is not strong enough");
			}
			await _userManager.CreateAsync(user, userRequest.Password);
			var administrator = new Administrator { UserId = user.Id };
			_coadaptService.Administrator.CreateAdministrator(administrator);
			await _coadaptService.SaveAsync();
			if (!await _roleManager.RoleExistsAsync(Role.AdministratorRole)) {
				await _roleManager.CreateAsync(new IdentityRole(Role.AdministratorRole));
			}
			await _userManager.AddToRoleAsync(user, Role.AdministratorRole);
			return CreatedAtRoute("AdministratorById", new { id = administrator.Id }, administrator);
		}

		/// <summary>
		/// Delete the administrator with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can delete an administrator
		/// - An administrator cannot delete oneself
		/// </remarks>
		/// <param name="id"></param>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteAdministrator(int id) {
			var administrator = await _coadaptService.Administrator.GetAdministratorByIdAsync(id);
			if (administrator.IsEmptyObject()) {
				_logger.LogDebug($"DeleteAdministrator: Administrator with ID {id} not found.");
				return NotFound("Administrator with requested ID does not exist");
			}
			string userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
			if (administrator.UserId == userId) {
				_logger.LogError($"DeleteAdministrator: An administrator cannot delete oneself.");
				return BadRequest("An administrator cannot delete oneself");
			}
			var user = await _userManager.FindByIdAsync(administrator.UserId);
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
			_coadaptService.Administrator.DeleteAdministrator(administrator);
			result = await _userManager.DeleteAsync(user);
			if (result != IdentityResult.Success) {
				return StatusCode(StatusCodes.Status500InternalServerError, "Cannot delete user");
			}
			await _coadaptService.SaveAsync();
			return NoContent();
		}

		/// <summary>
		/// Update an existing administrator with given ID
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - Only administrators can update an administrator
		/// - Username must be unique
		/// - Request an empty user name or password in order not to change them
		/// </remarks>
		/// <param name="id"></param>
		/// <param name="userRequest"></param>
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateAdministrator(int id, [FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("UpdateAdministrator: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("UpdateAdministrator: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			var administrator = await _coadaptService.Administrator.GetAdministratorByIdAsync(id);
			if (administrator.IsEmptyObject()) {
				_logger.LogError("UpdateAdministrator: Administrator with requested ID does not exist.");
				return NotFound("Administrator with requested ID does not exist");
			}
			if (userRequest.UserName == "" && userRequest.Password == "") {
				return NoContent();
			}
			var user = await _userManager.FindByIdAsync(administrator.UserId);
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
					_logger.LogError("UpdateAdministrator: Username already exists.");
					return BadRequest("Username already exists");
				}
				await _userManager.SetUserNameAsync(user, userRequest.UserName);
			}
			return NoContent();
		}

	}

}
