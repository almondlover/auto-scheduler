using AutoScheduler.Domain.DTOs.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class GeneratorRequirementsDTO
    {
        public required ActivityRequirementsDTO[] Requirements { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotDurationInMinutes { get; set; }
        //break length to be appended to slotbefore generation 
        public int BreakDurationInMinutes { get; set; } = 0;
    }
}
