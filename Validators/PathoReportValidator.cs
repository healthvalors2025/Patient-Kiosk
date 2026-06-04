using FluentValidation;
using PatientKiosk.Models;

namespace PatientKiosk.Validators
{
    public class PathoReportValidator : AbstractValidator<PathoReportRequestModel>
    {
        public PathoReportValidator() { 
            RuleFor(x => x.PatientIDF)
                .GreaterThan(0)
                .WithMessage("Patient ID is required.");
            RuleFor(x => x.HospitalIDF)
                .GreaterThan(0)
                .WithMessage("Hospital ID is required.");

        }
    }
}
