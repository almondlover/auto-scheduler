using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Timesheets
{
    public class Timeslot
    {
        public int Id { get; set; }
        public int TimesheetId { get; set; }
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; }
        public int HallId { get; set; }
        public int? MemberId { get; set; }
        public int? GroupId { get; set; }
        public Group? Group { get; set; }
        //should be enum
        public DayOfTheWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? OptimizationStatus { get; set; }
    }
}
