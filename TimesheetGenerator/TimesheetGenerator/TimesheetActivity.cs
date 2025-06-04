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
		public bool[][] PossibleHallsAvailability { get; set; }
		//pair of indx&length
		public List<int[]> PotentialSlots { get; set; }
		public List<TimesheetActivity> Children { get; set; } = new List<TimesheetActivity>();
		public TimesheetActivity Parent { get; set; }
		public void UpdateAvailability()
		{
			//should also eventually make more complex checks/modifications - maybe control presenter avail. chanegs through here
			//maybe find a way not to use terrible 3 nested for, but potential heavy refactoring
			PotentialSlots = new List<int[]>();
			for (int chunk=1; chunk <= ChunkCount; chunk++)
			{
				var nextChunkIdx = chunk * PresenterAvailability.Length / ChunkCount;
				for (int i = nextChunkIdx - PresenterAvailability.Length / ChunkCount; i < nextChunkIdx; i++)
				{
					int minNewSlotIdx = nextChunkIdx;
					int prevCount = PotentialSlots.Count;
					for (int j = 0; j < PossibleHallsAvailability.Length; j++)
					{
						int k = i;

						if (PotentialSlots.Any(slot => slot[0] + slot[1] > k && slot[2] == j))
							continue;
						//iterate potential free slot to the end so that overlappping slots are sorted by start index
						while (k < nextChunkIdx && !PossibleHallsAvailability[j][k] && !PresenterAvailability[k])
							k++;
						//save startidx, length & current hall idx if there's enough free spots to fit activity
						if (k - i > SlotCount)
						{
							PotentialSlots.Add(new int[] { i, k - i, j });
							if (k < minNewSlotIdx) minNewSlotIdx = k;
						}
					}
					//move to lowest index of newly inserted slots if any have been inserted
					i = prevCount < PotentialSlots.Count ? minNewSlotIdx : i;

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
