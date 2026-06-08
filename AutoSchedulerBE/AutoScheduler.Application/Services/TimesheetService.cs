using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Application.Entities.Mappers;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Interfaces.Repository;
using AutoScheduler.Domain.Interfaces.Service;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoMapper;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.DTOs;
using AutoScheduler.Application.Utils;

namespace AutoScheduler.Application.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _timesheetRepository;
        private IMapper _mapper;
        public TimesheetService(ITimesheetRepository timesheetRepository, IMapper mapper)
        {
            _timesheetRepository = timesheetRepository;
            _mapper = mapper;
        }
        public async Task CreateTimesheetAsync(TimesheetDTO timesheetDto)
        {
            var timesheet = _mapper.Map<Timesheet>(timesheetDto);
            await _timesheetRepository.CreateTimesheetAsync(timesheet);

            //create list of availabilities to update for halls & members
            var availabilityToAdd = timesheet.Timeslots?.Select(ts =>
            {
                return new Availability()
                {
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    DayOfTheWeek = ts.DayOfWeek,
                    MemberId = ts.MemberId,
                    HallId = ts.HallId
                };
            }).ToList();
            await _timesheetRepository.CreateAvailabilityRangeAsync(availabilityToAdd);
        }

        public async Task DeleteTimesheetAsync(int timesheetId)
        {
            await _timesheetRepository.DeleteTimesheetAsync(timesheetId);

            var timesheetToDeactivate = await _timesheetRepository.GetTimesheetByIdAsync(timesheetId);
            //delete availability entries corresponding to timeslots
            await _timesheetRepository.DeleteTimeslotsAvailability(timesheetToDeactivate.Timeslots);
        }
        private async Task<IList<TimesheetDTO>> TimesheetsFromGeneratorOutput(List<List<int[]>> generatorOutput, TimesheetGeneratorMapper mapper, int slotDuration)
        {
            var result = mapper.MapResult(generatorOutput);

            var allTimeslotsCollections = _mapper.Map<IList<TimeslotDTO[]>>(result);
            var timesheets = new List<TimesheetDTO>();

            foreach (var timeslots in allTimeslotsCollections)
            {
                var timesheetDto = new TimesheetDTO
                {
                    Id = 0,
                    Title = "",
                    BaseSlotDuration = slotDuration,
                    Timeslots = timeslots
                };
                timesheets.Add(timesheetDto);
            }

            return timesheets;

        }

        public async Task<IList<TimesheetDTO>> GenerateTimesheetAsync(GeneratorRequirementsDTO generatorRequirementsDTO)
        {
            
            var requirements = _mapper.Map<ActivityRequirements[]>(generatorRequirementsDTO.Requirements)
                                        .Select(req => { req.Duration += (req.Duration / generatorRequirementsDTO.SlotDurationInMinutes) * generatorRequirementsDTO.BreakDurationInMinutes; return req; }) //break time placeholder
                                        .OrderByDescending(req=>req.Duration)
                                        .ToArray();
            var finalSlotDureation = generatorRequirementsDTO.SlotDurationInMinutes + generatorRequirementsDTO.BreakDurationInMinutes;

            var halls = await _timesheetRepository.GetHallsForRequirementsAsync(requirements);
            var groups = await _timesheetRepository.GetGroupsForRequirementsAsync(requirements);
            var mapper = new TimesheetGeneratorMapper();
            var input = mapper.MapInput(requirements, groups.ToArray(), halls.ToArray(), generatorRequirementsDTO.StartTime, generatorRequirementsDTO.EndTime, finalSlotDureation);

            var timesheetGenerator = new TimesheetGenerator.TimesheetGenerator(input.TotalSlots, input.PresentersAvailability, input.HallsAvailability);
            timesheetGenerator.InitActivities(input.ActivityInput);
            timesheetGenerator.Generate();
            var generatorOutput = timesheetGenerator.Generated;

            return await TimesheetsFromGeneratorOutput(generatorOutput, mapper, finalSlotDureation);
        }

        public async Task<IList<WeekDayTimeRangeDTO>> GetAvailableSpaceForTimeslotAsync(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO)
        {
            //map entities
            var requirements = _mapper.Map<ActivityRequirements[]>(timeslotPlacementChangeDTO.GeneratorRequirements.Requirements)
                                        .Select(req => { req.Duration += (req.Duration / timeslotPlacementChangeDTO.GeneratorRequirements.SlotDurationInMinutes) * timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes; return req; }) //break time placeholder
                                        .OrderByDescending(req => req.Duration)
                                        .ToArray();
            var timeslot = _mapper.Map<Timeslot>(timeslotPlacementChangeDTO.ChangedTimeslot);
            var timeslotHall = _mapper.Map<Hall>(timeslotPlacementChangeDTO.ChangedTimeslot.Hall);

            //get index of requirement corresponding to this timeslot
            var timeslotRequirement = requirements.Where(r =>
                    r.ActivityId == timeslot.ActivityId
                    && r.Duration == (timeslot.EndTime - timeslot.StartTime).TotalMinutes
                    && r.MemberId == timeslot.MemberId
                    && r.Groups.Any(g => g.Id == timeslot.GroupId)
                    && r.HallTypeId == timeslotHall.HallTypeId
                ).FirstOrDefault();
            var timeslotReqIdx = Array.IndexOf(requirements, timeslotRequirement ?? new ActivityRequirements());

            //slot duration for generator slot should be slot dur. as per requirement + break
            var finalSlotDuration = timeslotPlacementChangeDTO.GeneratorRequirements.SlotDurationInMinutes + timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes;

            var halls = await _timesheetRepository.GetHallsForRequirementsAsync(requirements);
            var groups = await _timesheetRepository.GetGroupsForRequirementsAsync(requirements);

            if (timeslotReqIdx>-1) halls[timeslotReqIdx] = [timeslotHall];

            var generatorMapper = new TimesheetGeneratorMapper();
            var input = generatorMapper.MapInput(requirements, groups.ToArray(), halls.ToArray(), timeslotPlacementChangeDTO.GeneratorRequirements.StartTime, timeslotPlacementChangeDTO.GeneratorRequirements.EndTime, finalSlotDuration);

            int genActivityIndex = generatorMapper.IndexOfTimeslotActivity(timeslot);

            var timesheetGenerator = new TimesheetGenerator.TimesheetGenerator(input.TotalSlots, input.PresentersAvailability, input.HallsAvailability);
            timesheetGenerator.InitActivities(input.ActivityInput);

            var slots = timesheetGenerator.PotentialSlotsForActivity(genActivityIndex);

            return generatorMapper.MapTimeRanges(slots);
        }

        public async Task<IList<TimeslotDTO>> GetConflictingForTimeslot(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO)
        {
            //map entities
            var requirements = _mapper.Map<ActivityRequirements[]>(timeslotPlacementChangeDTO.GeneratorRequirements.Requirements)
                                        .Select(req => { req.Duration += (req.Duration / timeslotPlacementChangeDTO.GeneratorRequirements.SlotDurationInMinutes) * timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes; return req; }) //break time placeholder
                                        .OrderByDescending(req => req.Duration)
                                        .ToArray();
            var timeslot = _mapper.Map<Timeslot>(timeslotPlacementChangeDTO.ChangedTimeslot);
            var timeslotHall = _mapper.Map<Hall>(timeslotPlacementChangeDTO.ChangedTimeslot.Hall);
            var timeslotsForSheet = _mapper.Map<IList<Timeslot>>(timeslotPlacementChangeDTO.TimeslotsForSheet);

            if (timeslotPlacementChangeDTO.TimeslotsForSheet == null)
                return [];
            //remap for collection since its not in db
            for (int i =0; i< timeslotPlacementChangeDTO.TimeslotsForSheet.Count; i++)
            {
                timeslotsForSheet[i].Activity = _mapper.Map<Activity>(timeslotPlacementChangeDTO.TimeslotsForSheet[i].Activity);
                timeslotsForSheet[i].Member = _mapper.Map<Member>(timeslotPlacementChangeDTO.TimeslotsForSheet[i].Member);
                timeslotsForSheet[i].Hall = _mapper.Map<Hall>(timeslotPlacementChangeDTO.TimeslotsForSheet[i].Hall);
                timeslotsForSheet[i].Group = _mapper.Map<Group>(timeslotPlacementChangeDTO.TimeslotsForSheet[i].Group);
            }

            //slot duration for generator slot should be slot dur. as per requirement + break
            var finalSlotDuration = timeslotPlacementChangeDTO.GeneratorRequirements.SlotDurationInMinutes + timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes;

            var halls = await _timesheetRepository.GetHallsForRequirementsAsync(requirements);
            var groups = await _timesheetRepository.GetGroupsForRequirementsAsync(requirements);
            var generatorMapper = new TimesheetGeneratorMapper();
            generatorMapper.MapInput(requirements, groups.ToArray(), halls.ToArray(), timeslotPlacementChangeDTO.GeneratorRequirements.StartTime, timeslotPlacementChangeDTO.GeneratorRequirements.EndTime, finalSlotDuration);

            int genActivityIndex = generatorMapper.IndexOfTimeslotActivity(timeslot);
            generatorMapper.MapHallForActivity(genActivityIndex, timeslotHall);

            //remove targeted slot
            timeslotsForSheet.RemoveAt(((List<Timeslot>)timeslotsForSheet).FindIndex(ts => genActivityIndex == generatorMapper.IndexOfTimeslotActivity(ts)));
            var changedSlotInput = generatorMapper.MapSlotForGenerator(timeslot);
            var slotsInput = timeslotsForSheet.Select(ts => generatorMapper.MapSlotForGenerator(ts)).ToList();

            var timesheetGenerator = new TimesheetGenerator.TimesheetGenerator(generatorMapper.Input.TotalSlots, generatorMapper.Input.PresentersAvailability, generatorMapper.Input.HallsAvailability);
            timesheetGenerator.InitActivities(generatorMapper.Input.ActivityInput);

            var activityIndexes = timesheetGenerator.GetConflictingActivityIndexes(changedSlotInput, slotsInput);
            var result = activityIndexes.Select(i => timeslotsForSheet[slotsInput.FindIndex(s => s[1] == i)]).ToList();

            return _mapper.Map<IList<TimeslotDTO>>(result);
        }

        public Task<IList<Timesheet>> GetOptimizedTimesheetAsync(int timesheetId)
        {
            throw new NotImplementedException();
        }

        public async Task<TimesheetDTO> GetTimesheetByGroupIdAsync(int groupId)
        {
            return _mapper.Map<TimesheetDTO>(await _timesheetRepository.GetTimesheetByGroupIdAsync(groupId));
        }

        public async Task<Timesheet> GetTimesheetByIdAsync(int timesheetId)
        {
            return await _timesheetRepository.GetTimesheetByIdAsync(timesheetId);
        }

        public Task<IList<Timesheet>> GetTimesheetsForMemberAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Timeslot>> GetTimeslotsForMemberAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TimesheetDTO>> RegenerateTimesheetAsync(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO)
        {
            
            //map entities
            var requirements = _mapper.Map<ActivityRequirements[]>(timeslotPlacementChangeDTO.GeneratorRequirements.Requirements)
                                        .Select(req => { req.Duration += (req.Duration / timeslotPlacementChangeDTO.GeneratorRequirements.SlotDurationInMinutes) * timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes; return req; }) //break time placeholder
                                        .OrderByDescending(req => req.Duration)
                                        .ToList();
            var timeslot = _mapper.Map<Timeslot>(timeslotPlacementChangeDTO.ChangedTimeslot);
            var timeslotHall = _mapper.Map<Hall>(timeslotPlacementChangeDTO.ChangedTimeslot.Hall);
            timeslotHall.Type = _mapper.Map<HallType>(timeslotPlacementChangeDTO.ChangedTimeslot.Hall.Type);

            var timeslotRequirement = requirements.Where(r => 
                    r.ActivityId == timeslot.ActivityId
                    && r.Duration == (timeslot.EndTime - timeslot.StartTime).TotalMinutes
                    && r.MemberId == timeslot.MemberId
                    && r.Groups.Any(g => g.Id == timeslot.GroupId)
                    && r.HallTypeId == timeslotHall.HallTypeId
                ).FirstOrDefault();
            //move corresponmding requirement to beginning of array
            requirements.Remove(timeslotRequirement);
            var reqArray = requirements.Prepend(timeslotRequirement).ToArray();

            //slot duration for generator slot should be slot dur. as per requirement + break
            var finalSlotDuration = timeslotPlacementChangeDTO.GeneratorRequirements.SlotDurationInMinutes + timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes;

            var halls = await _timesheetRepository.GetHallsForRequirementsAsync(reqArray);
            //set single hall for timeslot
            halls[0] = [timeslotHall];

            var groups = await _timesheetRepository.GetGroupsForRequirementsAsync(reqArray);
            var generatorMapper = new TimesheetGeneratorMapper();
            var input = generatorMapper.MapInput(reqArray, groups.ToArray(), halls.ToArray(), timeslotPlacementChangeDTO.GeneratorRequirements.StartTime, timeslotPlacementChangeDTO.GeneratorRequirements.EndTime, finalSlotDuration);

            var changedSlotInput = generatorMapper.MapSlotForGenerator(timeslot);

            var timesheetGenerator = new TimesheetGenerator.TimesheetGenerator(input.TotalSlots, input.PresentersAvailability, input.HallsAvailability);
            timesheetGenerator.InitActivities(input.ActivityInput);

            //reserve selected slot
            timesheetGenerator.Generate(1, 1, [changedSlotInput]);
            var generatorOutput = timesheetGenerator.Generated;

            return await TimesheetsFromGeneratorOutput(generatorOutput, generatorMapper, finalSlotDuration);
        }

        public Task UpdateTimesheetAsync(Timesheet timesheet)
        {
            throw new NotImplementedException();
        }
    }
}
