using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class TimeslotPlacementChangeDTO
    {
        public required GeneratorRequirementsDTO GeneraRequirements { get; set; }
        public TimesheetDTO? FullTimesheet { get; set; }
        public required TimeslotDTO Timeslot { get; set; }
    }
}
