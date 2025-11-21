
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_Application_Part3.Models;
using Web_Application_Part3.Services;
using static System.Net.Mime.MediaTypeNames;

namespace Web_Application_Part3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ClaimService _claimService;

        public ClaimsController(ClaimService claimService)
        {
            _claimService = claimService;
        }

        [HttpGet("{claimId}")]
        public async Task<ActionResult<Claim>> GetClaimAsync(int claimId)
        {
            var claim = await _claimService.GetClaimAsync(claimId);
            return claim;
        }

        [HttpPost]
        public async Task<ActionResult> SubmitClaimAsync(Claim claim)
        {
            // Implement logic to submit claim
            return Ok();
        }

        [HttpPut("{claimId}/approve")]
        public async Task<ActionResult> ApproveClaimAsync(int claimId)
        {
            var claim = await _claimService.GetClaimAsync(claimId);
            if (claim == null)
            {
                return NotFound();
            }
            await _claimService.ApproveClaimAsync(claim);
            return Ok();
        }

        [HttpPut("{claimId}/decline")]
        public async Task<ActionResult> DeclineClaimAsync(int claimId)
        {
            var claim = await _claimService.GetClaimAsync(claimId);
            if (claim == null)
            {
                return NotFound();
            }
            await _claimService.DeclineClaimAsync(claim);
            return Ok();
        }
    }
}
