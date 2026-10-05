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
		private int _generalBreakDuration;
		private List<int> _memberEntityIds = new List<int>();
		private List<int> _hallEntityIds = new List<int>();
		private TimeOnly _startTime;
		private TimeOnly _endTime;
        private TimeOnly? _generalBreakStart;
        private ActivityRequirements[] _requirements;
		private List<GeneratorSlotProps> _slotProps = new List<GeneratorSlotProps>();
		private Group[] _groups;
		private IList<bool[]> _hallAvailability { get; set; }
        private IList<bool[]> _presenterAvailability { get; set; }
		public GeneratorMappingInput Input { get; private set; } = new GeneratorMappingInput();
        //might not need to be public? but could probably need to be fetched somewhere
        public int TotalSlotsPerChunk { get { return SlotDifference(_startTime, _endTime, _slotDurationMinutes, _generalBreakStart, _generalBreakDuration); } }
		public int GeneralBreakIdx { get { return SlotDifference(_startTime, _generalBreakStart ?? _startTime, _slotDurationMinutes); } }
        private int SlotDifference(TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes, TimeOnly? bigBreakStart = null, int bigBreakDuration = 0)
		{ 
			return ((int)(endTime - startTime).TotalMinutes - (endTime > bigBreakStart?.AddMinutes(bigBreakDuration) && startTime <= bigBreakStart?.AddMinutes(bigBreakDuration) ? bigBreakDuration : 0)) / slotDurationMinutes;
		}
		public GeneratorMappingInput MapInput(ActivityRequirements[] requirements,
			Group[] groups, 
			Hall[][] halls, 
			TimeOnly startTime, 
			TimeOnly endTime, 
			int slotDurationMinutes,
            TimeOnly? generalBreakStart = null,
            int generalBreakDuration = 0,
            Timeslot[]? reserved = null)
		{
			_requirements = requirements;
			_groups = groups;
			_slotDurationMinutes = slotDurationMinutes;
            _startTime = startTime;
            _endTime = endTime;
            _generalBreakDuration = generalBreakDuration;
			_generalBreakStart = generalBreakStart;

            //map requirements to helper class per group
            for (int i = 0; i < _requirements.Count(); i++)
			{
				if (requirements[i].CombineGroups)
                    _slotProps.Add(new GeneratorSlotProps
                    {
                        Member = _requirements[i].Member,
                        MemberId = _requirements[i].MemberId,
                        Activity = _requirements[i].Activity,
                        ActivityId = _requirements[i].ActivityId,
                        GroupIds = _requirements[i].Groups?.Select(g => g.Id).ToArray(),
                        Duration = _requirements[i].Duration,
                        Halls = halls[i]
                    });
                else for (int j = 0; j < _requirements[i].Groups.Count; j++)
                        {
                            _slotProps.Add(new GeneratorSlotProps
                            {
                                Member = _requirements[i].Member,
                                MemberId = _requirements[i].MemberId,
                                Activity = _requirements[i].Activity,
                                ActivityId = _requirements[i].ActivityId,
                                GroupIds = [_requirements[i].Groups?[j].Id ?? 0],
                                Duration = _requirements[i].Duration,
                                Halls = halls[i]
                            });
                        }
			}

			if (reserved != null)
			{
				//prepend activities for reserved slots
				//requirements should be unique
				var slotPropsForReservedSlots = reserved.Select(timeslot => _slotProps.Where(p =>
                    p.ActivityId == timeslot.ActivityId
                    && p.Duration == (timeslot.EndTime - timeslot.StartTime).TotalMinutes
                    && p.MemberId == timeslot.MemberId
					&& p.GroupIds.Contains(timeslot.GroupId ?? 0)//disregard halls as they could be overriden
                ).FirstOrDefault()).Where(sp => sp != null).ToList();

                foreach (var props in slotPropsForReservedSlots)
					_slotProps.Remove(props);
                _slotProps = slotPropsForReservedSlots.Concat(_slotProps).ToList();
            }

			int totalActivities = _slotProps.Count;

            //tf is this
            //_fullDailyDuration = (startTime - endTime).TotalMinutes;
            var durations = new int[totalActivities];
			var hallAvailability = new List<bool[]>();
			//num. of slots per day(chunk)
			int totalSlots = TotalSlotsPerChunk * _chunkCount;
			int generalBreakIdx = SlotDifference(startTime, generalBreakStart??startTime, slotDurationMinutes);

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
														+ SlotDifference(startTime,
															availSlot.StartTime < startTime ? startTime : availSlot.StartTime, 
															slotDurationMinutes,
                                                            generalBreakStart, generalBreakDuration);
							j < TotalSlotsPerChunk * (int)availSlot.DayOfTheWeek
														+ SlotDifference(startTime, 
															availSlot.EndTime > endTime ? endTime : availSlot.EndTime, 
															slotDurationMinutes,
                                                            generalBreakStart, generalBreakDuration);
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
				hallMapping[i] = new int[_slotProps[i].Halls.Length];
				//not sure how to simplify looping through available halls
				for (int k=0; k<_slotProps[i].Halls.Length; k++)
				{
					var currHallAvailability = _slotProps[i].Halls[k].Availability;
					var newHallAvailability = new bool[totalSlots];

					foreach (var availSlot in currHallAvailability)
					{
                        if (availSlot.EndTime < startTime || availSlot.StartTime > endTime) continue;
                        for (int j = TotalSlotsPerChunk * (int)availSlot.DayOfTheWeek
                                                        + SlotDifference(startTime, availSlot.StartTime < startTime ? startTime : availSlot.StartTime, slotDurationMinutes, generalBreakStart, generalBreakDuration);
                            j < TotalSlotsPerChunk * (int)availSlot.DayOfTheWeek
                                                        + SlotDifference(startTime, availSlot.EndTime > endTime ? endTime : availSlot.EndTime, slotDurationMinutes, generalBreakStart, generalBreakDuration);
                            j++)
                        {
							newHallAvailability[j] = true;
						}
					}
					
					int currHallIdx;
					if ((currHallIdx = hallEntityIds.IndexOf(_slotProps[i].Halls[k].Id)) > -1)
					{
						hallMapping[i][k] = currHallIdx;
					}
					else {
						hallEntityIds.Add(_slotProps[i].Halls[k].Id);
                        hallAvailability.Add(newHallAvailability);
						hallMapping[i][k] = hallAvailability.Count - 1;
					}
				}
			}
			var previousTypes = new List<ActivityType>();
            for (int i = 0; i < totalActivities; i++)
                parentMapping[i] = new List<int>();
            for (int i = 0; i < totalActivities; i++)
			{
				//need validation
				durations[i] = _slotProps[i].Duration / _slotDurationMinutes;
				

                if (_slotProps[i].Activity?.Type == null)
				{
					//need to check for duplicate groups in order to construct dependency graph properly & connecting duplicates
					//set parent to duplicate if it's past the current index => a chain of duplicates is constructed w/out breaking the tree
					var duplicateIdx = _slotProps.Skip(i + 1).ToList().FindIndex(prop => prop.GroupIds.SequenceEqual(_slotProps[i].GroupIds) && prop.Activity?.Type == null);
					if (duplicateIdx > -1)
					{
                        parentMapping[i].Add(duplicateIdx + i + 1);
						continue;
					}
				}
				else
				{
                    //find activity of same type within previous ones
					var duplicateIdx = _slotProps.Take(i).ToList().FindIndex(prop => prop.GroupIds.SequenceEqual(_slotProps[i].GroupIds)
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

                    //get index of first activity of different type for the same groups
                    duplicateIdx = _slotProps.Skip(i + 1).ToList().FindIndex(prop => prop.GroupIds.SequenceEqual(_slotProps[i].GroupIds)
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

					//get index of first activity for same groups w/out a type
                    duplicateIdx = _slotProps.FindIndex(prop => prop.GroupIds.SequenceEqual(_slotProps[i].GroupIds) && prop.Activity?.Type == null);

                    if (duplicateIdx > -1)
                    {
                        parentMapping[i].Add(duplicateIdx);
                        previousTypes.Add(_slotProps[i].Activity?.Type);
                        continue;
                    }
                }
				//find index of activity for min groups with subset of current groups
				int minInterecting = int.MaxValue;
				int minInterectedIdx = -1;
                var previousIntersectingIdxs = new List<int>();

				for (int j = 0; j < _slotProps.Count(); j++)
				{
					//skip if activity with same groups has been checked, could be optimized
					if (previousIntersectingIdxs.Any(idx => _slotProps[j].GroupIds.SequenceEqual(_slotProps[idx].GroupIds)))
						continue;
					var groupIntersection = _slotProps[j].GroupIds.Intersect(_slotProps[i].GroupIds);
					//skip if checked activity is subset of current/matching current/no intersection
					if (groupIntersection.Count() == _slotProps[j].GroupIds.Length || groupIntersection.Count() == 0)
						continue;

					if (groupIntersection.Count() != _slotProps[i].GroupIds.Length)
					{
                        parentMapping[i].Add(j);
						parentMapping[j].Add(i);
						previousIntersectingIdxs.Add(j);
                        continue;
                    }

					if (_slotProps[j].GroupIds.Length < minInterecting) 
                    {
						minInterecting = groupIntersection.Count();
                        minInterectedIdx = j;
                    }
				}
				if (minInterectedIdx > -1)
				//set act. w/ intersecting groups & min number of groups starting from first ocurrence
				{
                    //check activities with set types first as they are placed lower in the hierarchy
					for (int j = minInterectedIdx; j < _slotProps.Count(); j++)
					{
                        //skip if activity doesn't have a type or doesn't have appropriate number of groups or activity with same groups has been checked, could be optimized
                        if (_slotProps[j].GroupIds.Count() != minInterecting 
							|| previousIntersectingIdxs.Any(idx => _slotProps[j].GroupIds.SequenceEqual(_slotProps[idx].GroupIds))
							|| _slotProps[j].Activity?.Type == null)
                            continue;

                        var commonTypeProps = _slotProps.FindAll(prop => prop.GroupIds.SequenceEqual(_slotProps[j].GroupIds)
                                                                    && prop.Activity.ActivityTypeId != _slotProps[j].Activity.ActivityTypeId
                                                                    && prop.Activity.Type?.RootType().Id == _slotProps[j].Activity?.Type?.RootType().Id);
                        commonTypeProps.Add(_slotProps[j]);

                        foreach (var prop in commonTypeProps)
                        {
							int propIdx = _slotProps.IndexOf(prop);
							parentMapping[i].Add(propIdx);
							previousIntersectingIdxs.Add(propIdx);
						}
                    }
                    for (int j = minInterectedIdx; j < _slotProps.Count(); j++)
					{
						//skip if activity doesn't have appropriate number of groups or activity with same groups has been checked, could be optimized
						if (_slotProps[j].GroupIds.Count() != minInterecting 
							|| previousIntersectingIdxs.Any(idx => _slotProps[j].GroupIds.SequenceEqual(_slotProps[idx].GroupIds))
                            || _slotProps[j].Activity?.Type != null)
							continue;

						parentMapping[i].Add(j);
						previousIntersectingIdxs.Add(j);
					}
					continue;
				}

                //find index of parent group in requirements
                //only supported for multiple subgroups of same parent group
               var parentGroupIdx = Array.FindIndex(groups, grp => grp.Id == groups.FirstOrDefault(grp => grp.Id == _slotProps[i].GroupIds[0])?.ParentGroupId);
				//skip if parent group is not in collection
				if (parentGroupIdx < 0)
					continue; 

                var parentIdx = _slotProps.FindIndex(req => req.GroupIds[0] == groups[parentGroupIdx].Id && req.Activity?.Type != null);

				if (parentIdx > -1)
				{
					//get the other activities of same type
					var commonTypeProps = _slotProps.FindAll(prop => prop.GroupIds[0] == groups[parentGroupIdx].Id
																	&& prop.Activity.ActivityTypeId != _slotProps[parentIdx].Activity.ActivityTypeId
																	&& prop.Activity.Type?.RootType().Id == _slotProps[parentIdx].Activity?.Type?.RootType().Id);
                    commonTypeProps.Add(_slotProps[parentIdx]);

					foreach (var prop in commonTypeProps)
						parentMapping[i].Add(_slotProps.IndexOf(prop));

				}
				else 
				{
                    parentIdx = _slotProps.FindIndex(req => req.GroupIds[0] == groups[parentGroupIdx].Id && req.Activity?.Type == null);
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
				&& p.GroupIds.Contains(timeslot.GroupId ?? 0)
				&& p.Duration == SlotDifference(timeslot.StartTime, timeslot.EndTime, _slotDurationMinutes, _generalBreakStart, _generalBreakDuration) * _slotDurationMinutes); 
		}
		public void MapHallForActivity(int index, Hall hall)
		{
			int? hallIdx = null;
			for (int i =0; i < _slotProps.Count; i++)
			{
				var innerHallIdx = Array.FindIndex(_slotProps[i].Halls, h=>h.Id==hall.Id);
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
            _slotProps[index].Halls = [hall];
			Input.ActivityInput.HallMapping[index] = [hallIdx ?? _hallAvailability.Count - 1];
			Input.HallsAvailability = _hallAvailability.ToArray();
        }
		public int[] MapSlotForGenerator(Timeslot timeslot)
		{
			var index = IndexOfTimeslotActivity(timeslot);
			var hallIdx = Array.FindIndex(_slotProps[index].Halls, h => h.Id == timeslot.HallId);
			//calculate start index for slot & put activity index 
            return [
				(int)timeslot.DayOfWeek * TotalSlotsPerChunk + SlotDifference(_startTime, 
																			timeslot.StartTime, 
																			_slotDurationMinutes,
                                                                            _generalBreakStart, _generalBreakDuration),
				index,
                Input.ActivityInput.HallMapping[index][Array.FindIndex(_slotProps[index].Halls, h=>h.Id==timeslot.HallId)]
			];
		}
		public List<Hall> MapHallsFromOutput(List<int> generatorHallIdxs, int[] slot)
		{
			var result = new List<Hall>();
			foreach (var index in generatorHallIdxs)
			{
				var hall = _slotProps[slot[1]].Halls.FirstOrDefault(h => h.Id == _hallEntityIds[Input.ActivityInput.HallMapping[slot[1]][index]]);
				if (hall!=null)
					result.Add(hall);
			}
			return result;
        }
		public List<WeekDayTimeRangeDTO> MapTimeRanges(List<int[]> generatorOutput)
		{
			var timeRanges = new List<WeekDayTimeRangeDTO>();

			foreach (var slot in generatorOutput) 
			{
				int dayOfTheWeek = DayOfTheWeek(slot[0]);
				TimeOnly timeRangeStart = SlotStartTime(slot[0]);
				//add time for general break if it's inbetween start&end slots
				TimeOnly timeRangeEnd = timeRangeStart.AddMinutes(slot[1] * _slotDurationMinutes + (SlotInChunk(slot[0]) <= GeneralBreakIdx && SlotInChunk(slot[0]) + slot[1] > GeneralBreakIdx ? _generalBreakDuration : 0));

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
				var timeslots = new List<Timeslot>();
				for (int i = 0; i < generated.Count; i++)
				{
					//create separate timeslots for all groups
					for (int grpIdx = 0; grpIdx < _slotProps[generated[i][1]].GroupIds.Length; grpIdx++)
					{
						var newTimeslot = new Timeslot();
						//get the current day of the week(chunk) for this slot
						int dayOfWeek = DayOfTheWeek(generated[i][0]);
						newTimeslot.MemberId = _slotProps[generated[i][1]].MemberId;
						newTimeslot.Member = _slotProps[generated[i][1]].Member;
						newTimeslot.ActivityId = _slotProps[generated[i][1]].ActivityId;
						newTimeslot.Activity = _slotProps[generated[i][1]].Activity;
						newTimeslot.GroupId = _slotProps[generated[i][1]].GroupIds[grpIdx];
						newTimeslot.Group = _groups.First(group => group.Id == _slotProps[generated[i][1]].GroupIds[grpIdx]);
						newTimeslot.HallId = _hallEntityIds[generated[i][2]];
						//should be a better way to do this - maybe save mappings?
						foreach (var hallList in _slotProps.Select(sp => sp.Halls))
						{
							newTimeslot.Hall = hallList.FirstOrDefault(hall => hall.Id == _hallEntityIds[generated[i][2]]);
							if (newTimeslot.Hall != null)
								break;
						}
						newTimeslot.StartTime = SlotStartTime(generated[i][0]);
						newTimeslot.EndTime = newTimeslot.StartTime.AddMinutes(_slotProps[generated[i][1]].Duration 
											+ (newTimeslot.StartTime < _generalBreakStart && newTimeslot.StartTime.AddMinutes(_slotProps[generated[i][1]].Duration) > _generalBreakStart?.AddMinutes(_generalBreakDuration)
												? _generalBreakDuration : 0));
						newTimeslot.DayOfWeek = (DayOfTheWeek)dayOfWeek;
							newTimeslot.OptimizationStatus = "trust me bro";	
						timeslots.Add(newTimeslot);
					}
                }
				generatedTimesheets.Add(timeslots.ToArray());

            }

			return generatedTimesheets;
		}
		private int DayOfTheWeek(int generatorChunk)
		{
			return generatorChunk / TotalSlotsPerChunk;
        }
		private TimeOnly SlotStartTime(int generatorSlotIdx)
		{
			return _startTime.AddMinutes(_slotDurationMinutes * SlotInChunk(generatorSlotIdx) + (SlotInChunk(generatorSlotIdx) > GeneralBreakIdx ? _generalBreakDuration : 0));
        }
		private int SlotInChunk(int generatorSlotIdx)
		{
			return generatorSlotIdx % TotalSlotsPerChunk;

        }
    }
}
