using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class TimeslotRearrangementDTO
    {
        public required GeneratorRequirementsDTO GeneratorRequirements { get; set; }
        public required IList<TimeslotDTO> LockedTimeslots { get; set; }
    }
}
