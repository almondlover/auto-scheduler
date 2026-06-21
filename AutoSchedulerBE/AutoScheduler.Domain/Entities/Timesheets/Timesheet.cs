using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Timesheets
{
    public class Timesheet
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public TimesheetState State { get; set; }
        public bool Optimized { get; set; }
        public int BaseSlotDuration { get; set; }
        public int BreakDuration { get; set; }
        public IList<Timeslot>? Timeslots { get; set; }
        public IList<ActivityRequirements>? Requirements { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public TimeOnly? GeneralBreakStart { get; set; }
        public TimeOnly? GeneralBreakEnd { get; set; }
        public IList<TimesheetActivityRequirements>? TimesheetActivityRequirements { get; set; }

    }
}
