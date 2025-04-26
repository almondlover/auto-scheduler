using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Timesheets
{
    internal class Timeslot
    {
        public int Id { get; set; }
        public int TimesheetId { get; set; }
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; }
        public int HallId { get; set; }
        public int? MemberId { get; set; }
        public IList<Group>? Groups { get; set; }
        //should be enum
        public string? DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? OptimizationStatus { get; set; }
    }
}
