using AutoScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class TimesheetForMemberDTO
    {
        public int MemberId { get; set; }
        public required string MemberName { get; set; }
        public int SlotDuration { get; set; }
        public IList<TimeslotDTO>? Timeslots { get; set; }
    }
}
