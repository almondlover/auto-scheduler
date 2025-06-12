using AutoScheduler.Domain.Entities.Timesheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class TimesheetDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public bool Active { get; set; }
        public bool Optimized { get; set; }
        public IList<TimeslotDTO>? Timeslots { get; set; }
    }
}
