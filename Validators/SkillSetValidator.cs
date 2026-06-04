using FluentValidation;
using PatientKiosk.Models;

namespace PatientKiosk.Validators
{
    public class SkillSetValidator : AbstractValidator<SkillSetRequestModel>
    {
        public SkillSetValidator() { 
            
            RuleFor(x => x.HospitalGroupIDF)
                .GreaterThan(0)
                .WithMessage("Hospital Group ID is required.");
        }
    }
}
