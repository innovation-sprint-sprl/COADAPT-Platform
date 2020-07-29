using System;
using System.Threading.Tasks;
using Contracts.Logger;
using Contracts.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.WebAPI.Controllers {
	[Route("1.0/physiologicalInfo")]
	[ApiController]
	[AllowAnonymous]
	public class PhysiologicalInfoController : ControllerBase {
		private readonly ILoggerManager _logger;
		private readonly IRepositoryWrapper _coadaptService;

		public PhysiologicalInfoController(ILoggerManager logger, IRepositoryWrapper repository) {
			_logger = logger;
			_coadaptService = repository;
		}

		/// <summary>
		/// Receive a new message with encrypted physiological info
		/// </summary>
		/// <remarks>
		/// Remarks:
		/// - There is no need to have a logged-in user for COADAPT platform to receive encrypted physiological info
		/// </remarks>
		/// <param name="encryptedPhysiologicalInfo"></param>
		[HttpPost]
		[ProducesResponseType(typeof(String), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ReceiveEncryptedInfo([FromBody]String encryptedPhysiologicalInfo) {
			if (encryptedPhysiologicalInfo == null) {
				_logger.LogError("ReceiveEncryptedInfo: Encrypted string sent from client is null.");
				return BadRequest("Encrypted string is null");
			}
			return Ok("Encrypted info received");
		}

	}
}
