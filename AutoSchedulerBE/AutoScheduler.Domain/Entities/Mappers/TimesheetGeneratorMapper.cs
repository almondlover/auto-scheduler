using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Mappers
{
    public class TimesheetGeneratorMapper
    {
        private int _chunkCount = 5;
        private double _fullDailyDuration;
        private int _slotDurationMinutes;
        private int SlotDifference(TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes)
        { 
            return (int)_fullDailyDuration / slotDurationMinutes * _chunkCount;
        }
        public int[] MappActivities(ActivityRequirements[] requirements, Group[] groups, Hall[][] halls, TimeOnly startTime, TimeOnly endTime, int slotDurationMinutes)
        {
            _fullDailyDuration = (startTime - endTime).TotalMinutes;
            var durations = new int[requirements.Length];
            var hallAvailability = new List<bool[]>();
            int totalSlots = SlotDifference(startTime, endTime, slotDurationMinutes);

            List<bool[]> presenterAvailability = new List<bool[]>();
            List<bool[]> hallsAvailability = new List<bool[]>();
            
            List<int> presenterMapping = new List<int>();
            List<int[]> hallMapping = new List<int[]>();
            List<int> parentMapping = new List<int>();

            //should make query instead
            var memberIds = new List<int>();
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
                if ((currPresenterIdx = memberIds.IndexOf(requirements[i].MemberId))>-1) {
                    presenterMapping[i] = currPresenterIdx;
                }
                else {
                    memberIds.Add(requirements[i].MemberId);
                    presenterAvailability.Add(newPresenterAvailability);
                    presenterMapping[i] = presenterAvailability.Count;
                }
                
                var hallIds = new List<int>();
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
                    if ((currHallIdx = hallIds.IndexOf(halls[i][k].Id)) > -1)
                    {
                        hallMapping[i][k] = currHallIdx;
                    }
                    else {
                        hallAvailability.Add(newHallAvailability);
                        hallMapping[i][k] = hallAvailability.Count - 1;
                    }
                }


            }

            for (int i = 0; i < requirements.Length; i++)
            {
                durations[i] = requirements[i].Duration;

                //find index of parent group in requirements
                var parentGroupIdx = Array.FindIndex(groups, grp => grp.ParentGroupId == requirements[i].GroupId);
                if (parentGroupIdx < 0) continue;
                var parentIdx = parentGroupIdx < 0 ? parentGroupIdx : Array.FindIndex(requirements, req => req.GroupId == groups[parentGroupIdx].Id);
                parentMapping[i] = parentIdx;
            }
            return durations;
        }
    }
}
