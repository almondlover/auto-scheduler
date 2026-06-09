using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetGenerator
{
    public class TimesheetPreferences
    {
        public int StartSlot { get; set; }
        public int EndSlot { get; set; }
        public int ConsecutiveCount { get; set; }
        public int GapSize { get; set; }
        public int[] Chunks { get; set; }
    }
}
