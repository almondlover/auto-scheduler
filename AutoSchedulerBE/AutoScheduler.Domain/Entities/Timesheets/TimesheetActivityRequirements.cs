using AutoScheduler.Domain.Entities.Activities;

namespace AutoScheduler.Domain.Entities.Timesheets
{
    public class TimesheetActivityRequirements
    {
        public int TimesheetId { get; set; }
        public Timesheet? Timesheet { get; set; }
        public int ActivityRequirementsId { get; set; }
        public ActivityRequirements? Requirements { get; set; }
    }
}
