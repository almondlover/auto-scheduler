using System.Diagnostics;

namespace TimesheetGenerator
{
	public class TimesheetGenerator
	{
		private int[] _vacantSlots;
		private TimesheetActivity[] _activities;
		public List<List<int[]>> generated;
		private bool[][] _presentersAvailability;
        private bool[][] _hallsAvailability;
		private int[] _presenterMapping;
		private int[] _hallMapping;
        public TimesheetGenerator(int totalSlots, bool[][] presentersAvailability, bool[][] hallsAvailability)
		{
			_vacantSlots = new int[totalSlots];
			_presentersAvailability = presentersAvailability;
			_hallsAvailability = hallsAvailability;
        }
		public void InitActivities(int[] durations, int[] presenterMapping, int[] hallMapping)
		{
			_activities = new TimesheetActivity[durations.Length];
			_presenterMapping = presenterMapping;
			_hallMapping = hallMapping;
            for (int i=0; i < durations.Length; i++)
			{
				_activities[i] = new TimesheetActivity();
                _activities[i].SlotCount = durations[i];
				//maps presenter/hall availability to activity
				_activities[i].PresenterAvailability = _presentersAvailability[presenterMapping[i]];
                _activities[i].HallAvailability = _hallsAvailability[hallMapping[i]];
            }
		}
		public void Generate()
		{
			//pair of slot indx&activity indx
			var reservedSlots = new List<int[]>();
			generated = new List<List<int[]>>();

            ReserveSlots(0, reservedSlots, _activities, _presentersAvailability, _hallsAvailability);
		}
		private void ReserveSlots(int currentActivityIdx, List<int[]> reservedSlots, TimesheetActivity[] activities, bool[][] presentersAvailability, bool[][] hallsAvailability)
		{
			if (currentActivityIdx == _activities.Length)
			{
				//for (int i = 0; i < _vacantSlots.Length; i++)
				//	if (!_vacantSlots.Contains(i)) return;
				if (generated.Count<10) generated.Add(reservedSlots);
				return;
			}

            activities[currentActivityIdx].UpdateAvailability();

			int reservedIdx = 0;
            for (int i=0; i < activities[currentActivityIdx].PotentialSlots.Count; i++)
			{
				int[] newVacantSlots;

				//reserved slots need to be sorted by slot index
				//while (reservedIdx < reservedSlots.Count
    //                    && activities[currentActivityIdx].PotentialSlots[i][0] > reservedSlots[reservedIdx][0] + activities[reservedSlots[reservedIdx][1]].SlotCount)
				//	reservedIdx++;

				//iterate over the current available space for the activity
				int j = activities[currentActivityIdx].PotentialSlots[i][0];
				while (reservedIdx < reservedSlots.Count
						&& j > reservedSlots[reservedIdx][0]
						&& j + activities[currentActivityIdx].SlotCount < activities[currentActivityIdx].PotentialSlots[i][0] + activities[currentActivityIdx].PotentialSlots[i][1]
						&& activities[reservedSlots[reservedIdx][1]].AreConnected(activities[currentActivityIdx]))
				{
					//check next reserved slot
					j = Math.Max(j, reservedSlots[reservedIdx][0] + activities[reservedSlots[reservedIdx][1]].SlotCount);
					reservedIdx++;
					continue;
				}

				while (j + activities[currentActivityIdx].SlotCount < activities[currentActivityIdx].PotentialSlots[i][0] + activities[currentActivityIdx].PotentialSlots[i][1])
				{
					if (reservedIdx >= reservedSlots.Count
						|| j + activities[currentActivityIdx].SlotCount < reservedSlots[reservedIdx][0]
						|| !activities[reservedSlots[reservedIdx][1]].AreConnected(activities[currentActivityIdx]))
					{
						//clone collections
						List<int[]> newReservedSlots = reservedSlots.Select(slot => (int[])slot.Clone()).ToList();
						var newSlot = new int[] { j, currentActivityIdx };
						
						//insert new slot at idx to make sure list is sorted
						var insertIdx = newReservedSlots.FindIndex(slot => slot[0] > j);
						
						if (insertIdx<0) newReservedSlots.Add(newSlot);
						else newReservedSlots.Insert(newReservedSlots.FindIndex(slot => slot[0] > j), newSlot);

						//link avail. to activity
						bool[][] newHallsAvailability = hallsAvailability.Select(h =>
						{
							var newAvail = (bool[])h.Clone();
							return newAvail;
						}).ToArray();
						bool[][] newPresentersAvailability = presentersAvailability.Select(h =>
						{
							var newAvail = (bool[])h.Clone();
							return newAvail;
						}).ToArray();

                        TimesheetActivity[] newActivities = activities.Select((a, i) =>
                        {
                            var newAct = new TimesheetActivity()
                            {
                                PotentialSlots = a.PotentialSlots,
                                SlotCount = a.SlotCount,
                                //should probably be init elsewhere
                                HallAvailability = newHallsAvailability[_hallMapping[i]],
                                PresenterAvailability = newPresentersAvailability[_presenterMapping[i]]
                            };
                            return newAct;
                        }).ToArray();

                        //presenter/hall avail. should probably also be refactored or used as a property
                        for (int k = j; k < j + _activities[currentActivityIdx].SlotCount; k++)
						{
							newActivities[currentActivityIdx].HallAvailability[k] = true;
							newActivities[currentActivityIdx].PresenterAvailability[k] = true;
						}
						ReserveSlots(currentActivityIdx+1, newReservedSlots, newActivities, newPresentersAvailability, newHallsAvailability);
						j++;
					}
					else
					{
						//set index after current reserved slot
						j = reservedSlots[reservedIdx][0] + activities[reservedSlots[reservedIdx][1]].SlotCount;
						reservedIdx++;
					}
                }
			}
		}
	}
}
