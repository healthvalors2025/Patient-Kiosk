using FluentValidation;
using PatientKiosk.Models;

namespace PatientKiosk.Validators
{
    public class VerifyPatientOTPValidator : AbstractValidator<VerifyPatientOTPRequestModel>
    {
        public VerifyPatientOTPValidator()
        {
            RuleFor(x => x.KioskPatientOTPIDP)
                .GreaterThan(0)
                .WithMessage("OTP ID is required.");

            RuleFor(x => x.CRNumber)
                .NotEmpty()
                .WithMessage("CR Number is required.")
                .Length(9)
                .WithMessage("CR Number must be 9 digits.")
                .Matches(@"^\d+$")
                .WithMessage("CR Number must contain only digits.");

            RuleFor(x => x.KioskOTP)
                .NotEmpty()
                .WithMessage("OTP is required.")
                .Length(4)
                .WithMessage("OTP must be 4 digits.")
                .Matches(@"^\d+$")
                .WithMessage("OTP must contain only digits.");
        }
    }
}