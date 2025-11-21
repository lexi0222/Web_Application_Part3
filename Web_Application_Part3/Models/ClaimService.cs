
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web_Application_Part3.Data;
using Web_Application_Part3.Models;

namespace Web_Application_Part3.Services
{
    public class ClaimService
    {
        private readonly ClaimContext _context;

        public ClaimService(ClaimContext context)
        {
            _context = context;
        }

        public async Task<Claim> GetClaimAsync(int claimId)
        {
            return await _context.Claims.FindAsync(claimId);
        }

        public async Task<bool> VerifyClaimAsync(Claim claim)
        {
            // Implement logic to verify claim against predefined criteria
            return true; // Return true if claim is valid, false otherwise
        }

        public async Task ApproveClaimAsync(Claim claim)
        {
            // Implement logic to approve claim
            claim.claim_status = "Approved";
            await _context.SaveChangesAsync();
        }

        public async Task DeclineClaimAsync(Claim claim)
        {
            // Implement logic to decline claim
            claim.claim_status = "Declined";
            await _context.SaveChangesAsync();
        }
    }
}