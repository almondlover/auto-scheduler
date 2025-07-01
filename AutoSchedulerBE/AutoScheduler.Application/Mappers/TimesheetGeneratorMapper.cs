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
		private List<int> _memberEntityIds;
		private List<int> _hallEntityIds;
		private TimeOnly _startTime;
		private TimeOnly _endTime;
		private ActivityRequirements[] _requirements;
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
            //tf is this
            //_fullDailyDuration = (startTime - endTime).TotalMinutes;
            var durations = new int[requirements.Length];
			var hallAvailability = new List<bool[]>();
			//num. of slots per day(chunk)
			int totalSlots = TotalSlotsPerChunk * _chunkCount;

			List<bool[]> presenterAvailability = new List<bool[]>();
			List<bool[]> hallsAvailability = new List<bool[]>();
			
			int[] presenterMapping = new int[requirements.Length];
			int[][] hallMapping = new int[requirements.Length][];
			int[] parentMapping = new int[requirements.Length];

			//should make query instead
			var memberEntityIds = new List<int>();
            var hallEntityIds = new List<int>();
            var groupIds = new List<int>();
			for (int i=0; i<requirements.Length; i++)
			{
				groupIds.Add(requirements[i].GroupId??-1);

				var memberAvailability = requirements[i].Member?.Availability;
				var newPresenterAvailability = new bool[totalSlots];

				foreach (var availSlot in memberAvailability)
				{
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
				if ((currPresenterIdx = memberEntityIds.IndexOf(requirements[i].MemberId))>-1) {
					presenterMapping[i] = currPresenterIdx;
				}
				else {
					memberEntityIds.Add(requirements[i].MemberId);
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

			for (int i = 0; i < requirements.Length; i++)
			{
				//need validation
				durations[i] = requirements[i].Duration / _slotDurationMinutes;

				//need to check for duplicate groups in order to construct dependency graph properly & connecting duplicates
				//set parent to duplicate if it's past the current index => a chain of duplicates is constructed w/out breaking the tree
                var duplicateIdx = Array.FindIndex(requirements.Skip(i+1).ToArray(), req => req.GroupId == requirements[i].GroupId);
                if (duplicateIdx > -1)
                {
                    parentMapping[i] = duplicateIdx + i + 1;
                    continue;
                }

				//find index of parent group in requirements
				var parentGroupIdx = Array.FindIndex(groups, grp => grp.Id == groups.FirstOrDefault(grp => grp.Id == requirements[i].GroupId)?.ParentGroupId);
				//skip if parent group is not in collection
				if (parentGroupIdx < 0)
				{
					parentMapping[i] = -1;
					continue; 
				}
                var parentIdx = Array.FindIndex(requirements, req => req.GroupId == groups[parentGroupIdx].Id);
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
                    timeslots[i].MemberId = _requirements[generated[i][1]].MemberId;
                    timeslots[i].Member = _requirements[generated[i][1]].Member;
                    timeslots[i].ActivityId = _requirements[generated[i][1]].ActivityId;
                    timeslots[i].Activity = _requirements[generated[i][1]].Activity;
                    timeslots[i].GroupId = _requirements[generated[i][1]].GroupId ?? 0;
					timeslots[i].Group = _groups.First(group => group.Id == _requirements[generated[i][1]].GroupId);
                    timeslots[i].HallId = _hallEntityIds[generated[i][2]];
					//should be a better way to do this - maybe save mappings?
					foreach (var hallList in _halls)
					{
						timeslots[i].Hall = hallList.FirstOrDefault(hall => hall.Id == _hallEntityIds[generated[i][2]]);
						if (timeslots[i].Hall != null)
							break;
					}
                    timeslots[i].StartTime = _startTime.AddMinutes(_slotDurationMinutes * (generated[i][0] % TotalSlotsPerChunk));
					timeslots[i].EndTime = timeslots[i].StartTime.AddMinutes(_requirements[generated[i][1]].Duration);
					timeslots[i].DayOfWeek = (DayOfTheWeek)dayOfWeek;
					timeslots[i].OptimizationStatus = "trust me bro";	
                }
				generatedTimesheets.Add(timeslots);

            }

			return generatedTimesheets;
		}
	}
}
