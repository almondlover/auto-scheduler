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
        public bool[] PresenterAvailability { get; set; }
        public bool[] HallAvailability { get; set; }
        //pair of indx&length
        public List<int[]> PotentialSlots { get; set; }

        public void UpdateAvailability()
        {
            //should also eventually make more complex checks/modifications - maybe control presenter avail. chanegs through here
            PotentialSlots = new List<int[]>();
            for (int i=0; i < PresenterAvailability.Length; i++)
            {
                int consecutiveCount = 0;
                int j = i;
                while (j< PresenterAvailability.Length&&!HallAvailability[j] && !PresenterAvailability[j])
                {
                    consecutiveCount++;
                    j++;
                }
                //check if there's enough free spots to fit activity
                if (consecutiveCount > SlotCount)
                    PotentialSlots.Add(new int[] {i, consecutiveCount});
                i = j;
            }
        }
    }
}
