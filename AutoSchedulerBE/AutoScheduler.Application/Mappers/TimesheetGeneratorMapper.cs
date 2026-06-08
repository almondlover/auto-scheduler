using AutoScheduler.Application.Utils;
using AutoScheduler.Domain.DTOs;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Enums;
using AutoScheduler.Domain.Extensions;
using TimesheetGenerator;

namespace AutoScheduler.Application.Entities.Mappers
{
	public class TimesheetGeneratorMapper
	{
		private int _chunkCount = 5;
		private double _fullDailyDuration;
		private int _slotDurationMinutes;
		private List<int> _memberEntityIds = new List<int>();
		private List<int> _hallEntityIds = new List<int>();
		private TimeOnly _startTime;
		private TimeOnly _endTime;
		private ActivityRequirements[] _requirements;
		private List<GeneratorSlotProps> _slotProps = new List<GeneratorSlotProps>();
        private List<Hall[]> _halls;
		private Group[] _groups;
		private IList<bool[]> _hallAvailability { get; set; }
        private IList<bool[]> _presenterAvailability { get; set; }
		public GeneratorMappingInput Input { get; private set; } = new GeneratorMappingInput();
        //might not need to be public? but could probably need to be fetched somewhere
        public int TotalSlotsPerChunk { get { return SlotDifference(_startTime, _endTime, _slotDurationMinutes); } }
        private int SlotDifference(TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes)
		{ 
			return (int)(endTime - startTime).TotalMinutes / slotDurationMinutes;
		}
		public GeneratorMappingInput MapInput(ActivityRequirements[] requirements, Group[] groups, Hall[][] halls, TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes)
		{
			_requirements = requirements;
			_groups = groups;
			_slotDurationMinutes = slotDurationMinutes;
            _startTime = startTime;
            _endTime = endTime;
			_halls = new List<Hall[]>();

            //map requirements to helper class per group
            for (int i = 0; i < _requirements.Count(); i++)
			{
				for (int j=0; j < _requirements[i].Groups.Count; j++)
				{
					_slotProps.Add(new GeneratorSlotProps
                    {
                        Member = _requirements[i].Member,
                        MemberId = _requirements[i].MemberId,
                        Activity = _requirements[i].Activity,
                        ActivityId = _requirements[i].ActivityId,
                        GroupId = _requirements[i].Groups[j].Id,
                        Duration = _requirements[i].Duration
                    });
                    _halls.Add([..halls[i]]);
                }
			}

			int totalActivities = _slotProps.Count;

            //tf is this
            //_fullDailyDuration = (startTime - endTime).TotalMinutes;
            var durations = new int[totalActivities];
			var hallAvailability = new List<bool[]>();
			//num. of slots per day(chunk)
			int totalSlots = TotalSlotsPerChunk * _chunkCount;

			List<bool[]> presenterAvailability = new List<bool[]>();
			List<bool[]> hallsAvailability = new List<bool[]>();
			
			int[] presenterMapping = new int[totalActivities];
			int[][] hallMapping = new int[totalActivities][];
			List<int>[] parentMapping = new List<int>[totalActivities];

			//should make query instead
			var memberEntityIds = new List<int>();
            var hallEntityIds = new List<int>();
            var groupIds = new List<int>();
			for (int i=0; i< totalActivities; i++)
			{
				var memberAvailability = _slotProps[i].Member?.Availability;
				var newPresenterAvailability = new bool[totalSlots];

				foreach (var availSlot in memberAvailability)
				{
					//should maybe refactor to work w/ nighttime
					if (availSlot.EndTime < startTime || availSlot.StartTime > endTime) continue;
					for (int j = TotalSlotsPerChunk * (int)availSlot.DayOfTheWeek
														+ SlotDifference(startTime, availSlot.StartTime < startTime ? startTime : availSlot.StartTime, slotDurationMinutes);
							j < TotalSlotsPerChunk * (int)availSlot.DayOfTheWeek
														+ SlotDifference(startTime, availSlot.EndTime > endTime ? endTime : availSlot.EndTime, slotDurationMinutes);
							j++)
					{
						newPresenterAvailability[j] = true;
					}
				}
				
				int currPresenterIdx;
				//check if member has been added
				if ((currPresenterIdx = memberEntityIds.IndexOf(_slotProps[i].MemberId))>-1) {
					presenterMapping[i] = currPresenterIdx;
				}
				else {
					memberEntityIds.Add(_slotProps[i].MemberId);
					presenterAvailability.Add(newPresenterAvailability);
					presenterMapping[i] = presenterAvailability.Count-1;
				}

				//need to init hallmapping array first
				hallMapping[i] = new int[_halls[i].Length];
				//not sure how to simplify looping through available halls
				for (int k=0; k<_halls[i].Length; k++)
				{
					var currHallAvailability = _halls[i][k].Availability;
					var newHallAvailability = new bool[totalSlots];

					foreach (var availSlot in currHallAvailability)
					{
                        if (availSlot.EndTime < startTime || availSlot.StartTime > endTime) continue;
                        for (int j = TotalSlotsPerChunk * (int)availSlot.DayOfTheWeek
                                                        + SlotDifference(startTime, availSlot.StartTime < startTime ? startTime : availSlot.StartTime, slotDurationMinutes);
                            j < TotalSlotsPerChunk * (int)availSlot.DayOfTheWeek
                                                        + SlotDifference(startTime, availSlot.EndTime > endTime ? endTime : availSlot.EndTime, slotDurationMinutes);
                            j++)
                        {
							newHallAvailability[j] = true;
						}
					}
					
					int currHallIdx;
					if ((currHallIdx = hallEntityIds.IndexOf(_halls[i][k].Id)) > -1)
					{
						hallMapping[i][k] = currHallIdx;
					}
					else {
						hallEntityIds.Add(_halls[i][k].Id);
                        hallAvailability.Add(newHallAvailability);
						hallMapping[i][k] = hallAvailability.Count - 1;
					}
				}
			}
			var previousTypes = new List<ActivityType>();
			for (int i = 0; i < totalActivities; i++)
			{
				//need validation
				durations[i] = _slotProps[i].Duration / _slotDurationMinutes;
				parentMapping[i] = new List<int>();

                if (_slotProps[i].Activity?.Type == null)
				{
					//need to check for duplicate groups in order to construct dependency graph properly & connecting duplicates
					//set parent to duplicate if it's past the current index => a chain of duplicates is constructed w/out breaking the tree
					var duplicateIdx = _slotProps.Skip(i + 1).ToList().FindIndex(prop => prop.GroupId == _slotProps[i].GroupId && prop.Activity?.Type == null);
					if (duplicateIdx > -1)
					{
                        parentMapping[i].Add(duplicateIdx + i + 1);
						continue;
					}
				}
				else
				{
                    //find activity of same type within previous ones
					var duplicateIdx = _slotProps.Take(i).ToList().FindIndex(prop => prop.GroupId == _slotProps[i].GroupId
                                                                                                 && prop.Activity?.Type?.RootType().Id == _slotProps[i].Activity?.Type?.RootType().Id
                                                                                                 && prop.Activity?.ActivityTypeId != _slotProps[i].Activity?.ActivityTypeId);//should cover proper hierarchy?

                    if (duplicateIdx > -1)
                    {
                        //add parent activities of duplicate to current
						foreach (int idx in parentMapping[duplicateIdx])
                            parentMapping[i].Add(idx);

						//add current activity as parent for the same ones the duplicate is
						var childrenMapping = Array.FindAll(parentMapping, pm => pm == null ? false : pm.Any(idx => idx == duplicateIdx));

                        foreach (var idxList in childrenMapping)
                            idxList.Add(i);

                        continue;
                    }

                    //get index of first activity of different type for the same group
                    duplicateIdx = _slotProps.Skip(i + 1).ToList().FindIndex(prop => prop.GroupId == _slotProps[i].GroupId
																								 && prop.Activity?.Type != null
                                                                                                 && (prop.Activity?.Type?.RootType().Id != _slotProps[i].Activity?.Type?.RootType().Id
																								 || prop.Activity?.ActivityTypeId == _slotProps[i].Activity?.ActivityTypeId)//should cover proper hierarchy?
																								 && !previousTypes.Any(t => t.RootType().Id == prop.Activity?.Type?.RootType().Id
																															&& t.Id != prop.Activity?.ActivityTypeId));

                    if (duplicateIdx > -1)
                    {
                        parentMapping[i].Add(duplicateIdx + i + 1);
						previousTypes.Add(_slotProps[i].Activity?.Type);
                        continue;
                    }

					//get index of first activity for same group w/out a type
                    duplicateIdx = _slotProps.FindIndex(prop => prop.GroupId == _slotProps[i].GroupId && prop.Activity?.Type == null);

                    if (duplicateIdx > -1)
                    {
                        parentMapping[i].Add(duplicateIdx);
                        previousTypes.Add(_slotProps[i].Activity?.Type);
                        continue;
                    }
                }
				//find index of parent group in requirements
				var parentGroupIdx = Array.FindIndex(groups, grp => grp.Id == groups.FirstOrDefault(grp => grp.Id == _slotProps[i].GroupId)?.ParentGroupId);
				//skip if parent group is not in collection
				if (parentGroupIdx < 0)
					continue; 

                var parentIdx = _slotProps.FindIndex(req => req.GroupId == groups[parentGroupIdx].Id && req.Activity?.Type != null);

				if (parentIdx > -1)
				{
					//get the other activities of same type
					var commonTypeProps = _slotProps.FindAll(prop => prop.GroupId == groups[parentGroupIdx].Id
																	&& prop.Activity.ActivityTypeId != _slotProps[parentIdx].Activity.ActivityTypeId
																	&& prop.Activity.Type?.RootType().Id == _slotProps[parentIdx].Activity?.Type?.RootType().Id);
                    commonTypeProps.Add(_slotProps[parentIdx]);

					foreach (var prop in commonTypeProps)
						parentMapping[i].Add(_slotProps.IndexOf(prop));

				}
				else 
				{
                    parentIdx = _slotProps.FindIndex(req => req.GroupId == groups[parentGroupIdx].Id && req.Activity?.Type == null);
                    if (parentIdx > -1) 
						parentMapping[i].Add(parentIdx); 
				}
			}
			_memberEntityIds = memberEntityIds;
			_hallEntityIds = hallEntityIds;

            _hallAvailability = hallAvailability;
			_presenterAvailability = presenterAvailability;

            Input = new GeneratorMappingInput()
			{
				TotalSlots = totalSlots,
				PresentersAvailability = presenterAvailability.ToArray(),
				HallsAvailability = hallAvailability.ToArray(),
				ActivityInput = new ActivityInput()
				{
					Durations = durations,
					ChunkCount = _chunkCount,
					PresenterMapping = presenterMapping.ToArray(),
					HallMapping = hallMapping.ToArray(),
					ParentMapping = parentMapping.Select(pm => pm.ToArray()).ToArray()
				}
			};

			return Input;
		}
		public int IndexOfTimeslotActivity(Timeslot timeslot)
		{ 
			//halls are omitted as they will need to be turned to single hall
			//link between whole requirement & timeslot might still be needed
			return _slotProps.FindIndex(p => 
				p.ActivityId == timeslot.ActivityId
				&& p.MemberId == timeslot.MemberId
				&& p.GroupId == timeslot.GroupId
				&& p.Duration == (timeslot.EndTime - timeslot.StartTime).TotalMinutes); 
		}
		public void MapHallForActivity(int index, Hall hall)
		{
			int? hallIdx = null;
			for (int i =0; i < _halls.Count; i++)
			{
				var innerHallIdx = Array.FindIndex(_halls[i], h=>h.Id==hall.Id);
				if (innerHallIdx > -1)
				{
					hallIdx = Input.ActivityInput.HallMapping[i][innerHallIdx];
					break;
				}
			}
			//if there is no index for this hall add new
            if (hallIdx == null)
			{
				_hallAvailability.Add(new bool[TotalSlotsPerChunk * _chunkCount]);
                hallIdx = _hallAvailability.Count - 1;
            }
			Input.ActivityInput.HallMapping[index] = [hallIdx ?? _hallAvailability.Count - 1];
			Input.HallsAvailability = _hallAvailability.ToArray();
        }
		public int[] MapSlotForGenerator(Timeslot timeslot)
		{
			var index = IndexOfTimeslotActivity(timeslot);
			var hallIdx = Array.FindIndex(_halls[index], h => h.Id == timeslot.HallId);
			//calculate start index for slot & put activity index 
            return [
				(int)timeslot.DayOfWeek * TotalSlotsPerChunk + (int)(timeslot.StartTime - _startTime).TotalMinutes / _slotDurationMinutes,
				index,
				Array.FindIndex(_halls[index], h=>h.Id==timeslot.HallId)
			];
		}
		public List<WeekDayTimeRangeDTO> MapTimeRanges(List<int[]> generatorOutput)
		{
			var timeRanges = new List<WeekDayTimeRangeDTO>();

			foreach (var slot in generatorOutput) 
			{
				int dayOfTheWeek = DayOfTheWeek(slot[0]);
				TimeOnly timeRangeStart = SlotStartTime(slot[0]);
				TimeOnly timeRangeEnd = timeRangeStart.AddMinutes(slot[1] * _slotDurationMinutes);

                var timeRange = new WeekDayTimeRangeDTO { 
					StartTime = timeRangeStart,
					EndTime = timeRangeEnd,
					DayOfWeek = (DayOfTheWeek)dayOfTheWeek
                };

				timeRanges.Add(timeRange);
            }
			return timeRanges;
        }
		public List<Timeslot[]> MapResult(List<List<int[]>> generatorOutput)
		{
			List<Timeslot[]> generatedTimesheets = new List<Timeslot[]>();

			foreach (var generated in generatorOutput)
			{
				//convert generated list of reserved slots to timeslot entity
				var timeslots = new Timeslot[generated.Count].Select(timeslot=>new Timeslot()).ToArray();
				for (int i = 0; i < generated.Count; i++)
				{
					//get the current day of the week(chunk) for this slot
					int dayOfWeek = DayOfTheWeek(generated[i][0]);
                    timeslots[i].MemberId = _slotProps[generated[i][1]].MemberId;
                    timeslots[i].Member = _slotProps[generated[i][1]].Member;
                    timeslots[i].ActivityId = _slotProps[generated[i][1]].ActivityId;
                    timeslots[i].Activity = _slotProps[generated[i][1]].Activity;
                    timeslots[i].GroupId = _slotProps[generated[i][1]].GroupId ?? 0;
					timeslots[i].Group = _groups.First(group => group.Id == _slotProps[generated[i][1]].GroupId);
                    timeslots[i].HallId = _hallEntityIds[generated[i][2]];
					//should be a better way to do this - maybe save mappings?
					foreach (var hallList in _halls)
					{
						timeslots[i].Hall = hallList.FirstOrDefault(hall => hall.Id == _hallEntityIds[generated[i][2]]);
						if (timeslots[i].Hall != null)
							break;
					}
                    timeslots[i].StartTime = SlotStartTime(generated[i][0]);
					timeslots[i].EndTime = timeslots[i].StartTime.AddMinutes(_slotProps[generated[i][1]].Duration);
					timeslots[i].DayOfWeek = (DayOfTheWeek)dayOfWeek;
					timeslots[i].OptimizationStatus = "trust me bro";	
                }
				generatedTimesheets.Add(timeslots);

            }

			return generatedTimesheets;
		}
		private int DayOfTheWeek(int generatorChunk)
		{
			return generatorChunk / TotalSlotsPerChunk;
        }
		private TimeOnly SlotStartTime(int generatorSlotIdx)
		{
			return _startTime.AddMinutes(_slotDurationMinutes * (generatorSlotIdx % TotalSlotsPerChunk));
        }
	}
}
