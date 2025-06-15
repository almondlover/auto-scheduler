using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.DTOs.MemberGroups;
using AutoScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class TimeslotDTO
    {
        public int Id { get; set; }
        public int TimesheetId { get; set; }
        public required ActivityDTO Activity { get; set; }
        public required HallDTO Hall { get; set; }
        public required GroupDTO Group { get; set; }
        public required MemberDTO Member { get; set; }
        public DayOfTheWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
