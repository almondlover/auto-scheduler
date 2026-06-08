using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class TimeslotPlacementChangeDTO
    {
        public required GeneratorRequirementsDTO GeneratorRequirements { get; set; }
        public IList<TimeslotDTO>? TimeslotsForSheet { get; set; }
        public required TimeslotDTO ChangedTimeslot { get; set; }
    }
}
