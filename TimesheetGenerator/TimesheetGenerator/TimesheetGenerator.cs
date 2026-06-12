using System.Diagnostics;

namespace TimesheetGenerator
{
	public class TimesheetGenerator
	{
		private int[] _vacantSlots;
		private int _totalSlots;
		private int _totalChunks = 0;
		private TimesheetActivity[] _activities;
		public List<List<int[]>> Generated { get; set; }
		private List<Tuple<int, decimal>> _sortedMse = new List<Tuple<int, decimal>>();
		private int _capacity;
		private bool[][] _presentersAvailability;
		private bool[][] _hallsAvailability;
		private int[] _presenterMapping;
		private int[][] _hallMapping;
		private int[][] _parentMapping;
		private TimesheetPreferences _preferences;
		private int _minGapSize;
        private int _minConsecutiveSize;
        private int _minStartTime;
        private int _minEndTime;
        public TimesheetGenerator(int totalSlots, bool[][] presentersAvailability, bool[][] hallsAvailability, TimesheetPreferences? preferences = null)
		{
            _totalSlots = totalSlots;
			_presentersAvailability = presentersAvailability;
			_hallsAvailability = hallsAvailability;
			_preferences = preferences;
		}
		public void InitActivities(ActivityInput activityInput)
		{
			_activities = new TimesheetActivity[activityInput.Durations.Length];
			_presenterMapping = activityInput.PresenterMapping;
			_hallMapping = activityInput.HallMapping;
			_parentMapping = activityInput.ParentMapping;
			_totalChunks = activityInput.ChunkCount;
            for (int i=0; i < activityInput.Durations.Length; i++)
			{
				_activities[i] = new TimesheetActivity();
				_activities[i].ChunkCount = activityInput.ChunkCount;
				_activities[i].SlotCount = activityInput.Durations[i];
				//maps presenter/hall availability to activity
				_activities[i].PresenterAvailability = _presentersAvailability[activityInput.PresenterMapping[i]];
                _activities[i].PossibleHallsAvailability = new bool[activityInput.HallMapping[i].Length][];
				_activities[i].Parents = new List<TimesheetActivity>();
                for (int j=0; j < activityInput.HallMapping[i].Length; j++)
					_activities[i].PossibleHallsAvailability[j] = _hallsAvailability[activityInput.HallMapping[i][j]];
			}
			for (int i = 0; i < _activities.Length; i++)
			{
                foreach (var parentIdx in _parentMapping[i])
                {
                    _activities[i].Parents.Add(_activities[parentIdx]);
                    _activities[parentIdx].Children.Add(_activities[i]);
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
			if (Generated.Count == _capacity)
			{
				int slotsPerChunk = _totalSlots / _totalChunks;
				List<int> errors = new List<int>();
				for (int chunk = 0; chunk < _totalChunks; chunk++)
				{
					//slots already reserved that fall within the slot range of the current chunk
					//might already be sorted and just need to keep un index and iterate over them
					var reservedSlotsInChunk = reservedSlots.Where(s => s[0] + activities[s[2]].SlotCount < (chunk + 1) * slotsPerChunk
																		&& s[0] + activities[s[2]].SlotCount < chunk * slotsPerChunk).ToArray();

					foreach (var activity in activities)
						activity.UpdateAvailability();

					var activityChains = new List<TimesheetActivity[]>();
					//get ancestor chains only for leaf nodes
					foreach (var activity in activities.Where(a => a.Children.Count == 0))
					{
						var activityAnsestors = new List<TimesheetActivity>();
						activity.GetAncestors(activityAnsestors);
						activityChains.Add(activityAnsestors.ToArray());
					}

					foreach (var activityAncestors in activityChains)
					{
						//idx of last slot for any activity in ancestor list
						int lastSlotForActivitiesIdx = 0;
						for (int i = 1; i < reservedSlotsInChunk.Count(); i++)
						{
							if (!activityAncestors.Contains(activities[reservedSlotsInChunk[i][1]]))
								continue;

							//add gap size between subsequent slots for connected activities
							errors.Add(reservedSlotsInChunk[i][0] - reservedSlotsInChunk[i][0] - activities[reservedSlotsInChunk[i][1]].SlotCount);
							lastSlotForActivitiesIdx = i;
						}
					}

				}

				decimal meanSquaredError = errors.Sum(e => e * e) / errors.Count;

				if (meanSquaredError < _sortedMse.Last().Item2)
				{
					_sortedMse.RemoveAt(_sortedMse.Count);
					_sortedMse.Add(new Tuple<int, decimal>(Generated.Count, meanSquaredError));
					_sortedMse.OrderBy(i => i.Item2);
					Generated.RemoveAt(_sortedMse.Last().Item1);
					Generated.Add(reservedSlots);
				}

				//accuracy must be dynamic
				if (meanSquaredError < 0.2m)
					return; 
			}
			//stop if impossible to reserve slots for all activities
			if (reservedSlots.Count < currentActivityIdx) return;
			if (currentActivityIdx == _activities.Length)
			{
				Generated.Add(reservedSlots);
				return;
			}

			activities[currentActivityIdx].UpdateAvailability();

			int totalPresenterSlots = activities[currentActivityIdx].PresenterAvailability.Where(a => !a).Count();
			int presenterActivitiesSlots = activities.Skip(currentActivityIdx).Where((_, i) => _presenterMapping[i] == _presenterMapping[currentActivityIdx]).Sum(a => a.SlotCount);
			//stop if there aren't enough slots for all activities (without validating activity size)
			if (totalPresenterSlots < presenterActivitiesSlots) 
				return;

			int remainingConnectedSlotCount = activities[currentActivityIdx].ConnectedSlotCount(a => activities.Skip(currentActivityIdx).Contains(a));
			int totalRemainingSlotCount = _totalSlots - reservedSlots.Sum(r => activities[r[1]].SlotCount);
            //stop if there aren't enough slots for all activities (without validating activity size and availability)
            if (totalRemainingSlotCount < remainingConnectedSlotCount) 
				return;

            int reservedIdx = 0, lastReservedIdx = 0, lastPotentialSlotEnd = 0;
			for (int i=0; i < activities[currentActivityIdx].PotentialSlots.Count; i++)
			{
                //check if last potential slot overlaps with current one and if so go back to the first reserved idx before it
                //that way no reserved slots are missed
                if (lastPotentialSlotEnd > activities[currentActivityIdx].PotentialSlots[i][0])
                    reservedIdx = lastReservedIdx;
                else lastReservedIdx = reservedIdx;
                lastPotentialSlotEnd = activities[currentActivityIdx].PotentialSlots[i][0] + activities[currentActivityIdx].PotentialSlots[i][1];

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
							newActivities[k].Parents = new List<TimesheetActivity>();

                            foreach (var parentIdx in _parentMapping[k])
							{
								newActivities[k].Parents.Add(newActivities[parentIdx]);
								newActivities[parentIdx].Children.Add(newActivities[k]);
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
		public List<int[]> PotentialSlotsForActivity(int index)
		{
			_activities[index].UpdateAvailability();
			return _activities[index].PotentialSlots;
		}
		public List<int> GetConflictingActivityIndexes(int[] newSlot, List<int[]> reservedSlots)
		{
			var result = new List<int>();
			
			reservedSlots.RemoveAll(s => s[1] == newSlot[1]);

			foreach (var reservedSlot in reservedSlots)
				if (newSlot[0] < reservedSlot[0] + _activities[reservedSlot[1]].SlotCount
					&& reservedSlot[0] < newSlot[0] + _activities[newSlot[1]].SlotCount
					&& (_activities[newSlot[1]].AreConnected(_activities[reservedSlot[1]])
						|| _hallMapping[newSlot[1]][newSlot[2]] == _hallMapping[reservedSlot[1]][reservedSlot[2]]
						|| _presenterMapping[newSlot[1]] == _presenterMapping[reservedSlot[1]]))
					result.Add(reservedSlot[1]);

			return result;
		}
	}
}
