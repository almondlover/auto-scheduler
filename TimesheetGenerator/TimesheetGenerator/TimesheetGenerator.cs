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
		private int[] _parentMapping;
		public TimesheetGenerator(int totalSlots, bool[][] presentersAvailability, bool[][] hallsAvailability)
		{
			_vacantSlots = new int[totalSlots];
			_presentersAvailability = presentersAvailability;
			_hallsAvailability = hallsAvailability;
		}
		public void InitActivities(int[] durations, int[] presenterMapping, int[] hallMapping, int[] parentMapping)
		{
			_activities = new TimesheetActivity[durations.Length];
			_presenterMapping = presenterMapping;
			_hallMapping = hallMapping;
			_parentMapping = parentMapping;
			for (int i=0; i < durations.Length; i++)
			{
				_activities[i] = new TimesheetActivity();
				_activities[i].SlotCount = durations[i];
				//maps presenter/hall availability to activity
				_activities[i].PresenterAvailability = _presentersAvailability[presenterMapping[i]];
				_activities[i].HallAvailability = _hallsAvailability[hallMapping[i]];
				
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
						var newSlot = new int[] { j, currentActivityIdx };
						
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
								//should probably be init elsewhere
								HallAvailability = newHallsAvailability[_hallMapping[i]],
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
							newActivities[currentActivityIdx].HallAvailability[k] = true;
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
