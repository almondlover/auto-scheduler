using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesheetGenerator
{
    internal class TimesheetActivity
    {
        public int SlotCount { get; set;  }
        //num. of chunks of continuous slots 
        public int ChunkCount { get; set; } = 1;
        public bool[] PresenterAvailability { get; set; }
        public bool[] HallAvailability { get; set; }
        //pair of indx&length
        public List<int[]> PotentialSlots { get; set; }
        public List<TimesheetActivity> Children { get; set; } = new List<TimesheetActivity>();
        public TimesheetActivity Parent { get; set; }
        public void UpdateAvailability()
        {
            //should also eventually make more complex checks/modifications - maybe control presenter avail. chanegs through here
            PotentialSlots = new List<int[]>();
            for (int chunk=1; chunk <= ChunkCount; chunk++)
            {
                var nextChunkIdx = chunk * PresenterAvailability.Length / ChunkCount;
                for (int i = nextChunkIdx - ChunkCount; i < nextChunkIdx; i++)
                {
                    int consecutiveCount = 0;
                    int j = i;
                    while (j < nextChunkIdx && !HallAvailability[j] && !PresenterAvailability[j])
                    {
                        consecutiveCount++;
                        j++;
                    }
                    //check if there's enough free spots to fit activity
                    if (consecutiveCount > SlotCount)
                        PotentialSlots.Add(new int[] { i, consecutiveCount });
                    i = j;
                }
            }
        }
        public bool AreConnected(TimesheetActivity other)
        {
            return IsAncestor(other) || IsDescendant(other);
        }
        public bool IsDescendant(TimesheetActivity other)
        {
            if (other == null || this == other) return false;

            for (int i = 0; i < other.Children.Count; i++)
            {
                if (ReferenceEquals(this, other.Children[i]))
                    return true;
                else if (IsDescendant(other.Children[i]))
                    return true;
            }
            return false;
        }
        public bool IsAncestor(TimesheetActivity other)
        {
            if (other == null || this == other) return false;
            
            if (ReferenceEquals(this, other.Parent))
                return true;
            else if (IsAncestor(other.Parent))
                return true;
            return false;
        }
    }
}
