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
        public TimesheetGenerator(int totalSlots, bool[][] presentersAvailability, bool[][] hallsAvailability)
		{
			_vacantSlots = new int[totalSlots];
			_presentersAvailability = presentersAvailability;
			_hallsAvailability = hallsAvailability;
        }
		public void InitActivities(int[] durations, int[] presenterMapping, int[] hallMapping)
		{
			_activities = new TimesheetActivity[durations.Length];
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
				for (int i = 0; i < _vacantSlots.Length; i++)
					if (!_vacantSlots.Contains(i)) return;
				if (generated.Count<10) generated.Add(reservedSlots);
				return;
			}

            activities[currentActivityIdx].UpdateAvailability();

            for (int i=0; i< activities[currentActivityIdx].PotentialSlots.Count && i < reservedSlots.Count-1; i++)
			{
				int[] newVacantSlots;

				//reserved slots need to be sorted by slot index
                if (reservedSlots[i][0] + activities[reservedSlots[i][1]].SlotCount < activities[currentActivityIdx].PotentialSlots[i][0]
					/*&& _activities[currentActivityIdx].PotentialSlots[i][0] + _activities[currentActivityIdx] < reservedSlots[i+1][0]*/)
				{
					//iterate over the current available space fo the activity
					int j = activities[currentActivityIdx].PotentialSlots[i][0];
					while (j + activities[currentActivityIdx].SlotCount < reservedSlots[i + 1][0]
							&& j + activities[currentActivityIdx].SlotCount < activities[currentActivityIdx].PotentialSlots[i][0] + activities[currentActivityIdx].PotentialSlots[i][1])
					{
						//clone collections
						List<int[]> newReservedSlots = (List<int[]>)reservedSlots.Select(slot => slot.Clone());
                        newReservedSlots.Add(new int[] { j, currentActivityIdx });

                        TimesheetActivity[] newActivities = activities.Select(a => {
							var newAct = new TimesheetActivity()
								{
									PotentialSlots = a.PotentialSlots,
									SlotCount = a.SlotCount
								};
							return newAct;
							}).ToArray();

						//link avail. to activity
                        bool[][] newHallsAvailability = hallsAvailability.Select(h => {
								var newAvail = (bool[])h.Clone();
								if (h == activities[currentActivityIdx].HallAvailability)
									newActivities[currentActivityIdx].HallAvailability = newAvail;
								return newAvail;
                            }).ToArray();
                        bool[][] newPresentersAvailability = presentersAvailability.Select(h => {
                            var newAvail = (bool[])h.Clone();
                            if (h == activities[currentActivityIdx].PresenterAvailability)
                                newActivities[currentActivityIdx].PresenterAvailability = newAvail;
                            return newAvail;
                        }).ToArray();

						//presenter/hall avail. should probably also be refactored or used as a property
						for (int k = j; k < j + _activities[currentActivityIdx].SlotCount; k++)
						{
							newActivities[currentActivityIdx].HallAvailability[k] = false;
                            newActivities[currentActivityIdx].PresenterAvailability[k] = false;
                        }
						ReserveSlots(currentActivityIdx++, newReservedSlots, newActivities, newPresentersAvailability, newHallsAvailability);
						j++;
                    }
				}
			}
		}
	}
}
