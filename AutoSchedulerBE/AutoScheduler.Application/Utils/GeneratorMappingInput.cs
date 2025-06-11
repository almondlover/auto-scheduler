using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetGenerator;

namespace AutoScheduler.Application.Utils
{
    public class GeneratorMappingInput
    {
        public int TotalSlots { get; set; }
        public bool[][] PresentersAvailability { get; set; }
        public bool[][] HallsAvailability { get; set; }
        public ActivityInput ActivityInput { get; set; }
    }
}
