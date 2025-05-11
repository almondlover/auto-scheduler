using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetGenerator
{
    internal class TimesheetActivity
    {
        public int SlotCount { get; }
        public bool[] PresenterAvailability { get; }
        public bool[] HallAvailability { get; }
        public bool[] PotentialSlots { get; }

    }
}
