using FluentValidation;
using PatientKiosk.Models;

namespace PatientKiosk.Validators
{
    public class GeneratePatientOTPValidator : AbstractValidator<GeneratePatientOTPRequestModel>
    {
        public GeneratePatientOTPValidator()
        {
            RuleFor(x => x.PatientIDF)
                .GreaterThan(0)
                .WithMessage("Patient ID is required.");

            RuleFor(x => x.HospitalIDF)
                .GreaterThan(0)
                .WithMessage("Hospital ID is required.");

            RuleFor(x => x.CRNumber)
                .NotEmpty()
                .WithMessage("CR Number is required.")
                .Length(9)
                .WithMessage("CR Number must be 9 digits.")
                .Matches(@"^\d+$")
                .WithMessage("CR Number must contain only digits.");

            RuleFor(x => x.MobileNo)
                .NotEmpty()
                .WithMessage("Mobile Number is required.")
                .Length(10)
                .WithMessage("Mobile Number must be 10 digits.")
                .Matches(@"^\d+$")
                .WithMessage("Mobile Number must contain only digits.");
        }
    }
}