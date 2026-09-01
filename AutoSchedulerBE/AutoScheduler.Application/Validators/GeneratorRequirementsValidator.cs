using AutoScheduler.Domain.DTOs.Timesheets;
using FluentValidation;

namespace AutoScheduler.Application.Validators
{
    public class GeneratorRequirementsValidator : AbstractValidator<GeneratorRequirementsDTO>
    {
        public GeneratorRequirementsValidator() 
        {
            RuleFor(req => req.EndTime)
                .GreaterThan(req => req.StartTime).WithMessage("Schedule can't end before it begins!")
                .Must(IsValidLength).WithMessage("Daily duration must be long enough to fit an exact number of slots!");
            RuleFor(req => req.SlotDurationInMinutes)
                .GreaterThan(0).WithMessage("Slot duration must be non-negative!")
                .Must(IsValidSlotLength).WithMessage("Slot duration must be long enough so activities can fit into an exact number of slots");
            RuleFor(req => req.BreakDurationInMinutes)
                .GreaterThan(0).WithMessage("Break duration must be non-negative!");
            RuleFor(req => req.GeneralBreakEndTime)
                .GreaterThan(req => req.GeneralBreakStartTime).WithMessage("General break can't end before it begins!")
                .When(req => req.GeneralBreakEndTime != null && req.GeneralBreakStartTime != null)
                .Must((req, breakEnd) => (breakEnd - req.GeneralBreakStartTime)?.TotalMinutes > req.BreakDurationInMinutes).WithMessage("General breaks must be longer than normal breaks!")
                .When(req => req.GeneralBreakEndTime != null && req.GeneralBreakStartTime != null);
        }
        private bool IsValidLength(GeneratorRequirementsDTO requirements, TimeOnly endTime)
        {
            int fullDuration = requirements.SlotDurationInMinutes + requirements.BreakDurationInMinutes;
            int generalBreakDuration = (int?)(requirements.GeneralBreakEndTime - requirements.GeneralBreakEndTime)?.TotalMinutes ?? 0;
            if (((endTime - requirements.StartTime).TotalMinutes - generalBreakDuration) % fullDuration != 0)
                return false;
            return true;
        }
        private bool IsValidSlotLength(GeneratorRequirementsDTO requirements, int slotDurationInMinutes)
        {
            //make sure activities can be made up of a discrete number of slots
            if (requirements.Requirements.Any(req => req.Duration % slotDurationInMinutes != 0))
                return false;
            return true;
        }
    }
}
