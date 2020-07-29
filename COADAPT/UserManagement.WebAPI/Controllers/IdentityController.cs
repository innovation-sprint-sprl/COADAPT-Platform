using Contracts.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiModels;
using Constants;
using Contracts.Repository;
using Entities.Models;

namespace UserManagement.WebAPI.Controllers {
	[Route("1.0/account")]
	[ApiController]
	[AllowAnonymous]
	public class IdentityController : Controller {
		private readonly ILoggerManager _logger;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IRepositoryWrapper _coadaptService;

		public IdentityController(ILoggerManager logger, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IRepositoryWrapper repository) {
			_logger = logger;
			_userManager = userManager;
			_roleManager = roleManager;
			_coadaptService = repository;
		}

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="userRequest"></param>
		[HttpPost]
		[Route("login")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Login([FromBody]UserRequest userRequest) {
			if (userRequest == null) {
				_logger.LogError("Login: userRequest object sent from client is null.");
				return BadRequest("userRequest object is null");
			}
			if (!ModelState.IsValid) {
				_logger.LogError("Login: Invalid userRequest object sent from client.");
				return BadRequest("Invalid userRequest object");
			}
			var user = await _userManager.FindByNameAsync(userRequest.UserName);
			if (user == null) {
				_logger.LogError("Login: User does not exist.");
				return BadRequest("User does not exist");
			}
			AppUsageLog appUsageLog;
			if (!await _userManager.CheckPasswordAsync(user, userRequest.Password)) {
				_logger.LogError("Login: Incorrect password.");
				appUsageLog = new AppUsageLog() {Message = $"Failed login attempt for user {userRequest.UserName}",
					Tag =  "IdentityController", UserId = 0, ReportedOn = DateTime.Now};
				_coadaptService.AppUsageLog.CreateAppUsageLog(appUsageLog);
				await _coadaptService.SaveAsync();
				return BadRequest("Incorrect password");
			}
			var roles = await _userManager.GetRolesAsync(user);

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("af4fXtXz3LJzjQVRKVlBjnXOo8tChET4");
			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity(new[] {
					new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim("id", user.Id),
					new Claim("role", string.Join(",", roles))
				}),
				Expires = DateTime.UtcNow.AddDays(3),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
			int userId;
			if (roles.Contains(Role.AdministratorRole)) {
				var coadaptUser = await _coadaptService.Administrator.GetAdministratorByUserIdAsync(user.Id);
				userId = coadaptUser.Id;
			} else if (roles.Contains(Role.SubAdministratorRole)) {
				var coadaptUser = await _coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(user.Id);
				userId = coadaptUser.Id;
			} else if (roles.Contains(Role.SupervisorRole)) {
				var coadaptUser = await _coadaptService.Supervisor.GetSupervisorByUserIdAsync(user.Id);
				userId = coadaptUser.Id;
			} else if (roles.Contains(Role.TherapistRole)) {
				var coadaptUser = await _coadaptService.Therapist.GetTherapistByUserIdAsync(user.Id);
				userId = coadaptUser.Id;
			} else {
				var coadaptUser = await _coadaptService.Participant.GetParticipantByUserIdAsync(user.Id);
				userId = coadaptUser.Id;
			}
			appUsageLog = new AppUsageLog() {Message = "User login", Tag =  "IdentityController",
				UserId = userId, ReportedOn = DateTime.Now};
			_coadaptService.AppUsageLog.CreateAppUsageLog(appUsageLog);
			await _coadaptService.SaveAsync();

			return Ok(new LoginResponse { Id = user.Id, UserName = user.UserName, Roles = roles, Token = token });
		}

	}
}
