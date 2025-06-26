using System.Diagnostics;

namespace TimesheetGenerator
{
	public class TimesheetGenerator
	{
		private int[] _vacantSlots;
		private TimesheetActivity[] _activities;
		public List<List<int[]>> Generated { get; set; }
		private int _capacity;
		private bool[][] _presentersAvailability;
		private bool[][] _hallsAvailability;
		private int[] _presenterMapping;
		private int[][] _hallMapping;
		private int[] _parentMapping;
		public TimesheetGenerator(int totalSlots, bool[][] presentersAvailability, bool[][] hallsAvailability)
		{
			_vacantSlots = new int[totalSlots];
			_presentersAvailability = presentersAvailability;
			_hallsAvailability = hallsAvailability;
		}
		public void InitActivities(ActivityInput activityInput)
		{
			_activities = new TimesheetActivity[activityInput.Durations.Length];
			_presenterMapping = activityInput.PresenterMapping;
			_hallMapping = activityInput.HallMapping;
			_parentMapping = activityInput.ParentMapping;
			for (int i=0; i < activityInput.Durations.Length; i++)
			{
				_activities[i] = new TimesheetActivity();
				_activities[i].ChunkCount = activityInput.ChunkCount;
				_activities[i].SlotCount = activityInput.Durations[i];
				//maps presenter/hall availability to activity
				_activities[i].PresenterAvailability = _presentersAvailability[activityInput.PresenterMapping[i]];
                _activities[i].PossibleHallsAvailability = new bool[activityInput.HallMapping[i].Length][];
                for (int j=0; j < activityInput.HallMapping[i].Length; j++)
					_activities[i].PossibleHallsAvailability[j] = _hallsAvailability[activityInput.HallMapping[i][j]];
			}
			for (int i = 0; i < _activities.Length; i++)
			{
				if (_parentMapping != null && _parentMapping[i] != -1)
				{
					_activities[i].Parent = _activities[_parentMapping[i]];
					_activities[_parentMapping[i]].Children.Add(_activities[i]);
				}
			}
		}
		public void Generate()
		{
			Generate(1);
        }
		public void Generate(int maxCount)
		{
            Generate(maxCount, 0, new List<int[]>());
        }
		public void Generate(int maxCount, int startIdx, List<int[]> alreadyReserved)
		{
			//pair of slot indx&activity indx
			var reservedSlots = alreadyReserved;
            Generated = new List<List<int[]>>();
			//probably shouldn't be controlled by the generation method - needs validation
			_capacity = maxCount;

			ReserveSlots(startIdx, reservedSlots, _activities, _presentersAvailability, _hallsAvailability);
		}
		private void ReserveSlots(int currentActivityIdx, List<int[]> reservedSlots, TimesheetActivity[] activities, bool[][] presentersAvailability, bool[][] hallsAvailability)
		{
			if (Generated.Count == _capacity) return;
			//stop if impossible to reserve slots for all activities
			if (reservedSlots.Count < currentActivityIdx) return;
			if (currentActivityIdx == _activities.Length)
			{
				Generated.Add(reservedSlots);
				return;
			}

			activities[currentActivityIdx].UpdateAvailability();

			int reservedIdx = 0, lastReservedIdx = 0, lastPotentialSlotEnd = 0;
			for (int i=0; i < activities[currentActivityIdx].PotentialSlots.Count; i++)
			{
				//iterate over the current available space for the activity
				int j = activities[currentActivityIdx].PotentialSlots[i][0];
				while (reservedIdx < reservedSlots.Count
						&& j > reservedSlots[reservedIdx][0]
						&& j + activities[currentActivityIdx].SlotCount < activities[currentActivityIdx].PotentialSlots[i][0] + activities[currentActivityIdx].PotentialSlots[i][1])
				{
					//check next reserved slot
					if (activities[reservedSlots[reservedIdx][1]].AreConnected(activities[currentActivityIdx])) j = Math.Max(j, reservedSlots[reservedIdx][0] + activities[reservedSlots[reservedIdx][1]].SlotCount);
					reservedIdx++;
				}
				//check if last potential slot overlaps with current one and if so go back to the first reserved idx before it
				//that way no reserved slots are missed
				if (lastPotentialSlotEnd > activities[currentActivityIdx].PotentialSlots[i][0])
					reservedIdx = lastReservedIdx;
                else lastReservedIdx = reservedIdx;
				lastPotentialSlotEnd = activities[currentActivityIdx].PotentialSlots[i][0] + activities[currentActivityIdx].PotentialSlots[i][1];

                while (j + activities[currentActivityIdx].SlotCount < activities[currentActivityIdx].PotentialSlots[i][0] + activities[currentActivityIdx].PotentialSlots[i][1])
				{
					if (reservedIdx >= reservedSlots.Count
						|| j + activities[currentActivityIdx].SlotCount < reservedSlots[reservedIdx][0])
					{
						//clone collections
						List<int[]> newReservedSlots = reservedSlots.Select(slot => (int[])slot.Clone()).ToList();
						var newSlot = new int[] { j, currentActivityIdx, _hallMapping[currentActivityIdx][activities[currentActivityIdx].PotentialSlots[i][2]] };
						
						//insert new slot at idx to make sure list is sorted
						var insertIdx = newReservedSlots.FindIndex(slot => slot[0] > j);
						
						if (insertIdx<0) newReservedSlots.Add(newSlot);
						else newReservedSlots.Insert(insertIdx, newSlot);

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
								ChunkCount = a.ChunkCount,
                                //should probably be init elsewhere
                                PossibleHallsAvailability = _hallMapping[i].Select(j => newHallsAvailability[j]).ToArray(),
								PresenterAvailability = newPresentersAvailability[_presenterMapping[i]],
							};
							return newAct;
						}).ToArray();
						//need to copy activity references afterwards

						for (int k = 0; k<newActivities.Length; k++)
						{
							if (_parentMapping != null && _parentMapping[k] != -1)
							{
								newActivities[k].Parent = newActivities[_parentMapping[k]];
								newActivities[_parentMapping[k]].Children.Add(newActivities[k]);
							}
						}

						//presenter/hall avail. should probably also be refactored or used as a property
						for (int k = j; k < j + _activities[currentActivityIdx].SlotCount; k++)
						{
							newActivities[currentActivityIdx].PossibleHallsAvailability[activities[currentActivityIdx].PotentialSlots[i][2]][k] = true;
							newActivities[currentActivityIdx].PresenterAvailability[k] = true;
						}
						ReserveSlots(currentActivityIdx+1, newReservedSlots, newActivities, newPresentersAvailability, newHallsAvailability);
						j++;
					}
					else
					{
						//set index after current reserved slot
						if (activities[reservedSlots[reservedIdx][1]].AreConnected(activities[currentActivityIdx]))
							j = reservedSlots[reservedIdx][0] + activities[reservedSlots[reservedIdx][1]].SlotCount;
						reservedIdx++;
					}
				}
			}
		}
	}
}
