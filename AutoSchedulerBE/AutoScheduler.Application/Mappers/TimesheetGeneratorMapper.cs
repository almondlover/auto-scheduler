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
        private int SlotDifference(TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes)
		{ 
			return (int)(startTime - endTime).TotalMinutes / slotDurationMinutes * _chunkCount;
		}
		public GeneratorMappingInput MapInput(ActivityRequirements[] requirements, Group[] groups, Hall[][] halls, TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes)
		{
			_requirements = requirements;
			//tf is this
			//_fullDailyDuration = (startTime - endTime).TotalMinutes;
			var durations = new int[requirements.Length];
			var hallAvailability = new List<bool[]>();
			int totalSlots = SlotDifference(startTime, endTime, slotDurationMinutes) * _chunkCount;

			List<bool[]> presenterAvailability = new List<bool[]>();
			List<bool[]> hallsAvailability = new List<bool[]>();
			
			List<int> presenterMapping = new List<int>();
			List<int[]> hallMapping = new List<int[]>();
			List<int> parentMapping = new List<int>();

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
					for (int j = availSlot.DayOfWeek * SlotDifference(startTime, availSlot.StartTime, slotDurationMinutes); j < availSlot.DayOfWeek * SlotDifference(startTime, availSlot.EndTime, slotDurationMinutes); j++)
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
					presenterMapping[i] = presenterAvailability.Count;
				}
				
				//not sure how to simplify looping through available halls
				for (int k=0; k<halls[i].Length; k++)
				{
					var currHallAvailability = halls[i][k].Availability;
					var newHallAvailability = new bool[totalSlots];

					foreach (var availSlot in currHallAvailability)
					{
						for (int j = availSlot.DayOfWeek * SlotDifference(startTime, availSlot.StartTime, slotDurationMinutes); j < availSlot.DayOfWeek * SlotDifference(startTime, availSlot.EndTime, slotDurationMinutes); j++)
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

				//find index of parent group in requirements
				var parentGroupIdx = Array.FindIndex(groups, grp => grp.ParentGroupId == requirements[i].GroupId);
				if (parentGroupIdx < 0) continue;
				var parentIdx = parentGroupIdx < 0 ? parentGroupIdx : Array.FindIndex(requirements, req => req.GroupId == groups[parentGroupIdx].Id);
				parentMapping[i] = parentIdx;
			}
			_memberEntityIds = memberEntityIds;
			_hallEntityIds = hallEntityIds;
			_startTime = startTime;
			_endTime = endTime;

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
				var timeslot = new Timeslot();
				for (int i = 0; i < generated.Count; i++)
				{
					//get the current day of the week(chunk) for this slot
					int dayOfWeek = generated[i][0] / SlotDifference(_startTime, _endTime, _slotDurationMinutes);
					timeslot.MemberId = _requirements[generated[i][1]].MemberId;
					timeslot.ActivityId = _requirements[generated[i][1]].ActivityId;
					timeslot.GroupId = _requirements[generated[i][1]].GroupId ?? 0;
                    timeslot.HallId = _hallEntityIds[generated[i][2]];
					timeslot.StartTime = _startTime.AddMinutes(_slotDurationMinutes * generated[i][0] % dayOfWeek);
					timeslot.EndTime = _startTime.AddMinutes(_requirements[generated[i][1]].Duration);
					timeslot.DayOfWeek = (DayOfTheWeek)dayOfWeek;
					timeslot.OptimizationStatus = "trust me bro";	
                }

			}

			return generatedTimesheets;
		}
	}
}
