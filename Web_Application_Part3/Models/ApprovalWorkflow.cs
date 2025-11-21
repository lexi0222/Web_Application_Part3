
using FluentValidation;
using Web_Application_Part3.Models;

namespace Web_Application_Part3.Workflows
{
    public class ApprovalWorkflow
    {
        public void Execute(Claim claim)
        {
            // Define the approval workflow
            var validator = new ClaimValidator();
            var result = validator.Validate(claim);

            if (result.IsValid)
            {
                // Approve the claim
                claim.claim_status = "Approved";
            }
            else
            {
                // Decline the claim
                claim.claim_status = "Declined";
            }
        }
    }

    public class ClaimValidator : AbstractValidator<Claim>
    {
        public ClaimValidator()
        {
            RuleFor(c => c.FacallyName).NotEmpty().WithMessage("Facally name is required");
            RuleFor(c => c.ModuleName).NotEmpty().WithMessage("Module name is required");
            RuleFor(c => c.Hourworked).GreaterThan(0).WithMessage("Hours worked must be greater than 0");
            RuleFor(c => c.Hourlyrate).GreaterThan(0).WithMessage("Hourly rate must be greater than 0");
        }
    }
}

