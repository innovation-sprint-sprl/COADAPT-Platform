using Contracts.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
		
		private string GenerateRefreshToken() {
			var randomNumber = new byte[32];
			var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		private JWToken CreateJWT(IdentityUser user, IEnumerable<string> roles) {
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("af4fXtXz3LJzjQVRKVlBjnXOo8tChET4");
			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity(new[] {
					new Claim(JwtRegisteredClaimNames.Sub, user.UserName), 
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
					new Claim("id", user.Id), new Claim("role", string.Join(",", roles))
				}), 
				Expires = DateTime.UtcNow.AddHours(8), 
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			return new JWToken {
				Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)),
				RefreshToken = GenerateRefreshToken(),
				Expires = tokenDescriptor.Expires
			};
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
				appUsageLog = new AppUsageLog { Message = $"Failed login attempt for user {userRequest.UserName}", Tag = "IdentityController", UserId = 0, ReportedOn = DateTime.Now };
				_coadaptService.AppUsageLog.CreateAppUsageLog(appUsageLog);
				await _coadaptService.SaveAsync();
				return BadRequest("Incorrect password");
			}

			var roles = await _userManager.GetRolesAsync(user);

			var jwt = CreateJWT(user, roles);

			var userId = await _coadaptService.GetCoadaptUserIdByRole(user.Id, roles);
			appUsageLog = new AppUsageLog { Message = "User login", Tag = "IdentityController", UserId = userId, ReportedOn = DateTime.Now };
			_coadaptService.AppUsageLog.CreateAppUsageLog(appUsageLog);

			await _coadaptService.UserAccessToken.UpsertRefreshTokenAsync(user.Id, jwt.RefreshToken);
			await _coadaptService.SaveAsync();

			return Ok(new LoginResponse {
				Id = user.Id,
				UserName = user.UserName,
				Roles = roles,
				Token = jwt.Token,
				Expires = jwt.Expires,
				RefreshToken = jwt.RefreshToken
			});
		}

		/// <summary>
		/// Refresh JWT by providing a refreshToken.
		/// </summary>
		/// <param name="request"></param>
		[HttpPost]
		[Route("refreshToken")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequest request) {
			if (!ModelState.IsValid) return BadRequest();

			var oldToken = await _coadaptService.UserAccessToken.GetRefreshToken(request.RefreshToken);
			if (oldToken == null) return Unauthorized();

			var user = await _userManager.FindByIdAsync(oldToken.UserId);
			var roles = await _userManager.GetRolesAsync(user);

			var jwt = CreateJWT(user, roles);
			var refreshToken = GenerateRefreshToken();

			await _coadaptService.UserAccessToken.UpsertRefreshTokenAsync(user.Id, refreshToken);
			await _coadaptService.SaveAsync();

			return Ok(new LoginResponse {
				Id = user.Id,
				UserName = user.UserName,
				Roles = roles,
				Token = jwt.Token,
				Expires = jwt.Expires,
				RefreshToken = jwt.RefreshToken
			});
		}

	}

}
