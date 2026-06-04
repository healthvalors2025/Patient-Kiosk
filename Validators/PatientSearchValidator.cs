using FluentValidation;
using PatientKiosk.Models;

namespace PatientKiosk.Validators
{
    public class PatientSearchValidator : AbstractValidator<PatientSearchModel>
    {
        public PatientSearchValidator() {

            RuleFor(x => x.HospitalIDF)
           .GreaterThan(0)
           .WithMessage("Hospital ID is required.");

            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.MobileNo) ||
                    !string.IsNullOrWhiteSpace(x.ABHANo) ||
                    !string.IsNullOrWhiteSpace(x.CRNo))
                .WithMessage("At least one of Mobile No, ABHA No, or CR No is required.");

            RuleFor(x => x.MobileNo)
             .Matches(@"^\d{10}$")
             .When(x => !string.IsNullOrWhiteSpace(x.MobileNo))
             .WithMessage("Mobile No must be 10 digits.");

            RuleFor(x => x.CRNo)
                .Matches(@"^\d{9}$")
                .When(x => !string.IsNullOrWhiteSpace(x.CRNo))
                .WithMessage("CR No must be 9 digits.");
        }
    }
}
