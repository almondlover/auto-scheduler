using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Application.Entities.Mappers;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Interfaces.Repository;
using AutoScheduler.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoMapper;

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
        public Task CreateTimesheetAsync(Timesheet timesheet)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTimesheetAsync(int timesheetId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Timeslot[]>> GenerateTimesheetAsync(GeneratorRequirementsDTO generatorRequirementsDTO)
        {
            var requirements = _mapper.Map<ActivityRequirements[]>(generatorRequirementsDTO.Requirements);
            var halls = await _timesheetRepository.GetHallsForRequirementsAsync(requirements);
            var groups = await _timesheetRepository.GetGroupsForRequirementsAsync(requirements);
            var mapper = new TimesheetGeneratorMapper();
            var input = mapper.MapInput(requirements, groups.ToArray(), halls.ToArray(), generatorRequirementsDTO.StartTime, generatorRequirementsDTO.EndTime, generatorRequirementsDTO.SlotDurationInMinutes);

            var timesheetGenerator = new TimesheetGenerator.TimesheetGenerator(input.TotalSlots, input.PresentersAvailability, input.HallsAvailability);
            timesheetGenerator.InitActivities(input.ActivityInput);
            timesheetGenerator.Generate();
            var generatorOutput = timesheetGenerator.Generated;

            var result = mapper.MapResult(generatorOutput);

            return result;
        }

        public Task<IList<Timesheet>> GetOptimizedTimesheetAsync(int timesheetId)
        {
            throw new NotImplementedException();
        }

        public Task<Timesheet> GetTimesheetByGroupIdAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public Task<Timesheet> GetTimesheetByIdAsync(int timesheetId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Timesheet>> GetTimesheetsForMemberAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Timeslot>> GetTimeslotsForMemberAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTimesheetAsync(Timesheet timesheet)
        {
            throw new NotImplementedException();
        }
    }
}
