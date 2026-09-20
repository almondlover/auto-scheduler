using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;

namespace AutoScheduler.Domain.Entities.Activities
{
    public class ActivityRequirements
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; }
        public IList<Group>? Groups { get; set; }
        public bool CombineGroups { get; set; }
        public IList<Hall>? Halls { get; set; }
        public int MemberId { get; set; }
        public Member? Member { get; set; }
        public int Duration { get; set; }
        public int HallSize { get; set; }
        public int? TimesPerWeek { get; set; }
        public int? HallTypeId { get; set; }
        public HallType? HallType { get; set; }
        public IList<ActivityRequirementsGroup>? RequirementsGroups { get; set; }
        public IList<ActivityRequirementsHall>? RequirementsHalls { get; set; }
        public IList<TimesheetActivityRequirements>? TimesheetRequirements { get; set; }
        public IList<Timesheet>? Timesheets { get; set; }
    }
}
