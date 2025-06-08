using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetGenerator
{
    public class ActivityInput
    {
        public int[] Durations { get; set; }
        public int ChunkCount { get; set; }
        public int[] PresenterMapping { get; set; }
        public int[][] HallMapping { get; set; }
        public int[] ParentMapping { get; set; }
    }
}
