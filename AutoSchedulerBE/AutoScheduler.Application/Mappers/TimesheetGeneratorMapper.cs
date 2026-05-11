using AutoScheduler.Application.Utils;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Hall[][] _halls;
		private Group[] _groups;
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
			_halls = halls;
			_slotDurationMinutes = slotDurationMinutes;
            _startTime = startTime;
            _endTime = endTime;

			//map requirements to helper class per group
			foreach (var req in requirements)
			{
                _slotProps.AddRange(
					req.Groups?.Select(g => new GeneratorSlotProps
                    {
						Member = req.Member,
						MemberId = req.MemberId,
						Activity = req.Activity,
						ActivityId = req.ActivityId,
						GroupId = g.Id,
						Duration = req.Duration
					}).ToList() ?? new List<GeneratorSlotProps>());
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
			int[] parentMapping = new int[totalActivities];

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
														+ SlotDifference(startTime, availSlot.StartTime<startTime?startTime:availSlot.StartTime, slotDurationMinutes);
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
				hallMapping[i] = new int[halls[i].Length];
				//not sure how to simplify looping through available halls
				for (int k=0; k<halls[i].Length; k++)
				{
					var currHallAvailability = halls[i][k].Availability;
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
					if ((currHallIdx = hallEntityIds.IndexOf(halls[i][k].Id)) > -1)
					{
						hallMapping[i][k] = currHallIdx;
					}
					else {
						hallEntityIds.Add(halls[i][k].Id);
                        hallAvailability.Add(newHallAvailability);
						hallMapping[i][k] = hallAvailability.Count - 1;
					}
				}
			}

			for (int i = 0; i < totalActivities; i++)
			{
				//need validation
				durations[i] = _slotProps[i].Duration / _slotDurationMinutes;

				//need to check for duplicate groups in order to construct dependency graph properly & connecting duplicates
				//set parent to duplicate if it's past the current index => a chain of duplicates is constructed w/out breaking the tree
                var duplicateIdx = Array.FindIndex(_slotProps.Skip(i+1).ToArray(), req => req.GroupId == _slotProps[i].GroupId);
                if (duplicateIdx > -1)
                {
                    parentMapping[i] = duplicateIdx + i + 1;
                    continue;
                }

				//find index of parent group in requirements
				var parentGroupIdx = Array.FindIndex(groups, grp => grp.Id == groups.FirstOrDefault(grp => grp.Id == _slotProps[i].GroupId)?.ParentGroupId);
				//skip if parent group is not in collection
				if (parentGroupIdx < 0)
				{
					parentMapping[i] = -1;
					continue; 
				}
                var parentIdx = _slotProps.FindIndex(req => req.GroupId == groups[parentGroupIdx].Id);
				parentMapping[i] = parentIdx;
			}
			_memberEntityIds = memberEntityIds;
			_hallEntityIds = hallEntityIds;

			return new GeneratorMappingInput()
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
					ParentMapping = parentMapping.ToArray()
				}
			};
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
					int dayOfWeek = generated[i][0] / TotalSlotsPerChunk;
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
                    timeslots[i].StartTime = _startTime.AddMinutes(_slotDurationMinutes * (generated[i][0] % TotalSlotsPerChunk));
					timeslots[i].EndTime = timeslots[i].StartTime.AddMinutes(_slotProps[generated[i][1]].Duration);
					timeslots[i].DayOfWeek = (DayOfTheWeek)dayOfWeek;
					timeslots[i].OptimizationStatus = "trust me bro";	
                }
				generatedTimesheets.Add(timeslots);

            }

			return generatedTimesheets;
		}
	}
}
