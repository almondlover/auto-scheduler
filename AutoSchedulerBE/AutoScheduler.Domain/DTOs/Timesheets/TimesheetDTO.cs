using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.Enums;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class TimesheetDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public TimesheetState State { get; set; }
        public bool Optimized { get; set; }
        public int BaseSlotDuration { get; set; }
        public int BreakDuration { get; set; }
        public IList<TimeslotDTO>? Timeslots { get; set; }
        public IList<ActivityRequirementsDTO>? Requirements { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public TimeOnly? GeneralBreakStart { get; set; }
        public TimeOnly? GeneralBreakEnd { get; set; }
    }
}
