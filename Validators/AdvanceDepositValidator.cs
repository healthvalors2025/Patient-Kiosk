using FluentValidation;
using PatientKiosk.Models;

namespace PatientKiosk.Validators
{
    public class AdvanceDepositValidator : AbstractValidator<AdvanceDepositModel>
    {
        public AdvanceDepositValidator() { 
        
            RuleFor(x => x.PatientIDF)
                .GreaterThan(0)
                .WithMessage("Patient ID is required.");
            RuleFor(x => x.HospitalIDF)
                .GreaterThan(0)
                .WithMessage("Hospital ID is required.");
            RuleFor(x => x.AdvanceAmount)
                .GreaterThan(0)
                .WithMessage("Advance Amount must be greater than zero.");
            RuleFor(x => x.ModeOfPaymentIDF)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Mode of Payment ID is required.");
            RuleFor(x => x.Kiosk_UserIDF)
                .GreaterThan(0)
                .WithMessage("Kiosk User ID is required.");
        }
    }
}
