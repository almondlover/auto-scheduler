using AutoMapper;
using AutoScheduler.Application.Entities.Mappers;
using AutoScheduler.Domain.DTOs;
using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Enums;
using AutoScheduler.Domain.Interfaces.Repository;
using AutoScheduler.Domain.Interfaces.Service;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace AutoScheduler.Application.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _timesheetRepository;
        private IMapper _mapper;
        private IValidator<GeneratorRequirementsDTO> _generatorRequirementsValidator;
        public TimesheetService(ITimesheetRepository timesheetRepository, IMapper mapper, IValidator<GeneratorRequirementsDTO> generatorRequirementsValidator)
        {
            _timesheetRepository = timesheetRepository;
            _mapper = mapper;
            _generatorRequirementsValidator = generatorRequirementsValidator;
        }
        public async Task<TimesheetDTO> CreateTimesheetAsync(TimesheetDTO timesheetDto)
        {
            var timesheet = _mapper.Map<Timesheet>(timesheetDto);
            timesheet.State = TimesheetState.Draft;
            await _timesheetRepository.CreateTimesheetAsync(timesheet);
            return _mapper.Map<TimesheetDTO>(await _timesheetRepository.GetTimesheetByIdAsync(timesheet.Id));        
        }

        public async Task DeleteTimesheetAsync(int timesheetId)
        {
            var timesheetToDelete = await _timesheetRepository.GetTimesheetByIdAsync(timesheetId);
            //disallow direct deletion of active sheets
            if (timesheetToDelete.State == TimesheetState.Active)
                throw new InvalidOperationException();
            //permanently delete sheet
            await _timesheetRepository.DeleteTimesheetAsync(timesheetId);
            //delete availability entries corresponding to timeslots
            await _timesheetRepository.DeleteTimeslotsAvailability(timesheetToDelete.Timeslots);
        }
        private async Task<IList<TimesheetDTO>> TimesheetsFromGeneratorOutput(List<List<int[]>> generatorOutput,
            TimesheetGeneratorMapper mapper,
            int slotDuration,
            int breakDuration,
            TimeOnly startTime,
            TimeOnly endTime,
            TimeOnly? generalBreakStart,
            TimeOnly? generalBreakEnd,
            ActivityRequirementsDTO[] requirements)
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
                    BreakDuration = breakDuration,
                    StartTime = startTime,
                    EndTime = endTime,
                    GeneralBreakStart = generalBreakStart,
                    GeneralBreakEnd = generalBreakEnd,
                    Requirements = requirements,
                    Timeslots = timeslots
                };
                timesheets.Add(timesheetDto);
            }

            return timesheets;

        }

        public async Task<IList<TimesheetDTO>> GenerateTimesheetAsync(GeneratorRequirementsDTO generatorRequirementsDTO)
        {
            var validationResult = _generatorRequirementsValidator.Validate(generatorRequirementsDTO);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var requirements = _mapper.Map<ActivityRequirements[]>(generatorRequirementsDTO.Requirements)
                                        .Select(req => { req.Duration += (req.Duration / generatorRequirementsDTO.SlotDurationInMinutes) * generatorRequirementsDTO.BreakDurationInMinutes; return req; }) //break time placeholder
                                        .OrderByDescending(req=>req.Duration)
                                        .ToArray();
            var finalSlotDureation = generatorRequirementsDTO.SlotDurationInMinutes + generatorRequirementsDTO.BreakDurationInMinutes;

            var halls = await _timesheetRepository.GetHallsForRequirementsAsync(requirements);
            var groups = await _timesheetRepository.GetGroupsForRequirementsAsync(requirements);
            var mapper = new TimesheetGeneratorMapper();
            var input = mapper.MapInput(requirements, groups.ToArray(), halls.ToArray(), 
                generatorRequirementsDTO.StartTime, generatorRequirementsDTO.EndTime, finalSlotDureation, 
                generatorRequirementsDTO.GeneralBreakStartTime, 
                (int)((generatorRequirementsDTO.GeneralBreakEndTime ?? generatorRequirementsDTO.StartTime) - (generatorRequirementsDTO.GeneralBreakStartTime ?? generatorRequirementsDTO.StartTime)).TotalMinutes - generatorRequirementsDTO.BreakDurationInMinutes);

            var timesheetGenerator = new TimesheetGenerator.TimesheetGenerator(input.TotalSlots, input.PresentersAvailability, input.HallsAvailability);
            timesheetGenerator.InitActivities(input.ActivityInput);
            timesheetGenerator.Generate();
            var generatorOutput = timesheetGenerator.Generated;

            return await TimesheetsFromGeneratorOutput(generatorOutput, mapper,
                generatorRequirementsDTO.SlotDurationInMinutes,
                generatorRequirementsDTO.BreakDurationInMinutes,
                generatorRequirementsDTO.StartTime,
                generatorRequirementsDTO.EndTime,
                generatorRequirementsDTO.GeneralBreakStartTime,
                generatorRequirementsDTO.GeneralBreakEndTime,
                generatorRequirementsDTO.Requirements);
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


            int generalBreakDuration = (int)((timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakEndTime ?? timeslotPlacementChangeDTO.GeneratorRequirements.StartTime) - (timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakStartTime ?? timeslotPlacementChangeDTO.GeneratorRequirements.StartTime)).TotalMinutes - timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes;
            //get index of requirement corresponding to this timeslot
            var timeslotRequirement = requirements.Where(r =>
                    r.ActivityId == timeslot.ActivityId
                    && r.Duration == (timeslot.EndTime - timeslot.StartTime).TotalMinutes - (timeslot.EndTime > timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakEndTime && timeslot.StartTime < timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakEndTime ? generalBreakDuration : 0)
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
            var input = generatorMapper.MapInput(requirements, groups.ToArray(), halls.ToArray(), 
                timeslotPlacementChangeDTO.GeneratorRequirements.StartTime, timeslotPlacementChangeDTO.GeneratorRequirements.EndTime, finalSlotDuration,
                timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakStartTime,
                (int)((timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakEndTime ?? timeslotPlacementChangeDTO.GeneratorRequirements.StartTime) - (timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakStartTime ?? timeslotPlacementChangeDTO.GeneratorRequirements.StartTime)).TotalMinutes - timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes);

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
            generatorMapper.MapInput(requirements, groups.ToArray(), halls.ToArray(),
                timeslotPlacementChangeDTO.GeneratorRequirements.StartTime, timeslotPlacementChangeDTO.GeneratorRequirements.EndTime, finalSlotDuration,
                timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakStartTime,
                (int)((timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakEndTime ?? timeslotPlacementChangeDTO.GeneratorRequirements.StartTime) - (timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakStartTime ?? timeslotPlacementChangeDTO.GeneratorRequirements.StartTime)).TotalMinutes - timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes);

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
        public async Task<IList<HallDTO>> GetPossibleHallsForSlot(TimeslotPlacementChangeDTO timeslotPlacementChangeDTO)
        {
            //map entities
            var requirements = _mapper.Map<ActivityRequirements[]>(timeslotPlacementChangeDTO.GeneratorRequirements.Requirements)
                                        .Select(req => { req.Duration += (req.Duration / timeslotPlacementChangeDTO.GeneratorRequirements.SlotDurationInMinutes) * timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes; return req; }) //break time placeholder
                                        .OrderByDescending(req => req.Duration)
                                        .ToArray();
            var timeslot = _mapper.Map<Timeslot>(timeslotPlacementChangeDTO.ChangedTimeslot);

            //slot duration for generator slot should be slot dur. as per requirement + break
            var finalSlotDuration = timeslotPlacementChangeDTO.GeneratorRequirements.SlotDurationInMinutes + timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes;

            var halls = await _timesheetRepository.GetHallsForRequirementsAsync(requirements);
            var groups = await _timesheetRepository.GetGroupsForRequirementsAsync(requirements);
            var generatorMapper = new TimesheetGeneratorMapper();
            generatorMapper.MapInput(requirements, groups.ToArray(), halls.ToArray(),
                timeslotPlacementChangeDTO.GeneratorRequirements.StartTime, timeslotPlacementChangeDTO.GeneratorRequirements.EndTime, finalSlotDuration,
                timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakStartTime,
                (int)((timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakEndTime ?? timeslotPlacementChangeDTO.GeneratorRequirements.StartTime) - (timeslotPlacementChangeDTO.GeneratorRequirements.GeneralBreakStartTime ?? timeslotPlacementChangeDTO.GeneratorRequirements.StartTime)).TotalMinutes - timeslotPlacementChangeDTO.GeneratorRequirements.BreakDurationInMinutes);

            int genActivityIndex = generatorMapper.IndexOfTimeslotActivity(timeslot);

            var changedSlotInput = generatorMapper.MapSlotForGenerator(timeslot);

            var timesheetGenerator = new TimesheetGenerator.TimesheetGenerator(generatorMapper.Input.TotalSlots, generatorMapper.Input.PresentersAvailability, generatorMapper.Input.HallsAvailability);
            timesheetGenerator.InitActivities(generatorMapper.Input.ActivityInput);

            var hallIndexes = timesheetGenerator.PotentialHallsForSlot(changedSlotInput);
            var result = generatorMapper.MapHallsFromOutput(hallIndexes, changedSlotInput);

            return _mapper.Map<IList<HallDTO>>(result);
        }

        public Task<IList<Timesheet>> GetOptimizedTimesheetAsync(int timesheetId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TimesheetDTO>> GetTimesheetByGroupIdAsync(int groupId, TimesheetState state)
        {
            return _mapper.Map<IList<TimesheetDTO>>(await _timesheetRepository.GetTimesheetByGroupIdAsync(groupId, state));
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

        public async Task<IList<TimesheetDTO>> RegenerateTimesheetAsync(TimeslotRearrangementDTO timeslotRearrangementDto)
        {
            //slot duration for generator slot should be slot dur. as per requirement + break
            var finalSlotDuration = timeslotRearrangementDto.GeneratorRequirements.SlotDurationInMinutes + timeslotRearrangementDto.GeneratorRequirements.BreakDurationInMinutes;

            //map entities
            var requirements = _mapper.Map<ActivityRequirements[]>(timeslotRearrangementDto.GeneratorRequirements.Requirements)
                                        .Select(req => { req.Duration += (req.Duration / timeslotRearrangementDto.GeneratorRequirements.SlotDurationInMinutes) * timeslotRearrangementDto.GeneratorRequirements.BreakDurationInMinutes; return req; }) //break time placeholder
                                        .OrderByDescending(req => req.Duration)
                                        .ToArray();
            var lockedTimeslots = _mapper.Map<Timeslot[]>(timeslotRearrangementDto.LockedTimeslots);
            var timeslotHalls = _mapper.Map<Hall[]>(timeslotRearrangementDto.LockedTimeslots.Select(ts => ts.Hall).ToArray());
            //need to properly map halltype dtos
            for (int i = 0; i<timeslotHalls.Length; i++) timeslotHalls[i].Type = _mapper.Map<HallType>(timeslotRearrangementDto.LockedTimeslots[i].Hall.Type);

            var halls = await _timesheetRepository.GetHallsForRequirementsAsync(requirements);
            

            var groups = await _timesheetRepository.GetGroupsForRequirementsAsync(requirements);
            var generatorMapper = new TimesheetGeneratorMapper();
            var input = generatorMapper.MapInput(requirements, groups.ToArray(), halls.ToArray(),
                timeslotRearrangementDto.GeneratorRequirements.StartTime, timeslotRearrangementDto.GeneratorRequirements.EndTime, finalSlotDuration,
                timeslotRearrangementDto.GeneratorRequirements.GeneralBreakStartTime,
                (int)((timeslotRearrangementDto.GeneratorRequirements.GeneralBreakEndTime ?? timeslotRearrangementDto.GeneratorRequirements.StartTime) - (timeslotRearrangementDto.GeneratorRequirements.GeneralBreakStartTime ?? timeslotRearrangementDto.GeneratorRequirements.StartTime)).TotalMinutes, lockedTimeslots);
            
            //set single halls for locked in timeslots
            for (int i =0; i < timeslotHalls.Length; i++)
            {
                int genActivityIndex = generatorMapper.IndexOfTimeslotActivity(lockedTimeslots[i]);
                generatorMapper.MapHallForActivity(genActivityIndex, timeslotHalls[i]);
            }
            
            //map reserved slots for generator
            var lockedSlotsInput = new List<int[]>();
            foreach (var timeslot in lockedTimeslots)
                lockedSlotsInput.Add(generatorMapper.MapSlotForGenerator(timeslot));

            var timesheetGenerator = new TimesheetGenerator.TimesheetGenerator(input.TotalSlots, input.PresentersAvailability, input.HallsAvailability);
            timesheetGenerator.InitActivities(input.ActivityInput);

            //reserve selected slots
            //locked in slots list for generator should be same count as generator activities
            timesheetGenerator.Generate(1, lockedSlotsInput.Count, lockedSlotsInput);
            var generatorOutput = timesheetGenerator.Generated;

            return await TimesheetsFromGeneratorOutput(generatorOutput, 
                generatorMapper,
                timeslotRearrangementDto.GeneratorRequirements.SlotDurationInMinutes,
                timeslotRearrangementDto.GeneratorRequirements.BreakDurationInMinutes,
                timeslotRearrangementDto.GeneratorRequirements.StartTime,
                timeslotRearrangementDto.GeneratorRequirements.EndTime,
                timeslotRearrangementDto.GeneratorRequirements.GeneralBreakStartTime,
                timeslotRearrangementDto.GeneratorRequirements.GeneralBreakEndTime,
                timeslotRearrangementDto.GeneratorRequirements.Requirements);
        }

        public async Task UpdateTimesheetAsync(TimesheetDTO timesheetDto)
        {
            if (timesheetDto.State != TimesheetState.Draft)
                throw new InvalidOperationException("Can't change timesheet that's not a draft");
            
            var timesheet = _mapper.Map<Timesheet>(timesheetDto);
            await _timesheetRepository.UpdateTimesheetAsync(timesheet);
        }

        public async Task ActivateTimesheetAsync(int timesheetId)
        {
            var timesheet = await _timesheetRepository.GetTimesheetForUpdateAsync(timesheetId);

            var rootGroupIds = timesheet.Timeslots.Where(ts => !timesheet.Timeslots.Any(ts1 => ts.Group?.ParentGroupId == ts1.GroupId)).Select(ts => ts.GroupId).Distinct();
            //disallow multiple active timesheets for (main) group
            foreach (var id in rootGroupIds)
            {
                var timesheetForGroup = await _timesheetRepository.GetTimesheetByGroupIdAsync(id ?? 0, TimesheetState.Active);
                if (!timesheetForGroup.IsNullOrEmpty())
                    throw new InvalidOperationException("Main group already has an active timesheet");
            }
            
            //update timesheet state to active and save in db
            timesheet.State = TimesheetState.Active;
            await _timesheetRepository.UpdateTimesheetAsync(timesheet);
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

        public async Task DeactivateTimesheetAsync(int timesheetId)
        {
            var timesheetToDeactivate = await _timesheetRepository.GetTimesheetForUpdateAsync(timesheetId);

            if (timesheetToDeactivate == null || timesheetToDeactivate.Timeslots.IsNullOrEmpty())
                throw new InvalidOperationException();
            //delete availability entries corresponding to timeslots
            await _timesheetRepository.DeleteTimeslotsAvailability(timesheetToDeactivate.Timeslots);
            //set state to inactive as a soft delete
            timesheetToDeactivate.State = TimesheetState.Inactive;
            await _timesheetRepository.UpdateTimesheetAsync(timesheetToDeactivate);
        }

        public async Task<IList<ActivityRequirementsDTO>> GetRequirementsForTimesheetAsync(int timesheetId)
        {
            return _mapper.Map<IList<ActivityRequirementsDTO>>(await _timesheetRepository.GetRequirementsForTimesheetAsync(timesheetId));
        }
    }
}
