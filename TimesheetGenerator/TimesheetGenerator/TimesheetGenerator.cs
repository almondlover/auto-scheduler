using System.Diagnostics;

namespace TimesheetGenerator
{
	public class TimesheetGenerator
	{
		private int[] _vacantSlots;
		private TimesheetActivity[] _activities;
		public List<int[]> generated;
		public TimesheetGenerator(int totalSlots)
		{
			_vacantSlots = new int[totalSlots];
		}
		public void ReserveSlots(int currentActivityIdx, int[] vacantSlots)
		{
			if (currentActivityIdx == _activities.Length)
			{
				for (int i = 0; i < _vacantSlots.Length; i++)
					if (!_vacantSlots.Contains(i)) return;
				generated.Add(vacantSlots);
				return;
			}

			for (int i=0; i< vacantSlots.Length; i++)
			{
				int[] newVacantSlots;
                if (vacantSlots[i] == 0 && _activities[currentActivityIdx].PotentialSlots[i])
				{
					newVacantSlots = (int[])vacantSlots.Clone();
					int j;
					for (j=i; _activities[currentActivityIdx].PotentialSlots[j]&&j-1< _activities[currentActivityIdx].SlotCount; j++)
					{
						if (vacantSlots[j] == 0)
						{
							i = j;
							break;
						}
						//put activity in the free slots
						newVacantSlots[j] = currentActivityIdx;
                    }
					//stupid check
                    if (i!=j) ReserveSlots(currentActivityIdx++, newVacantSlots);
				}
			}
		}
	}
}
