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

namespace AutoScheduler.Application.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _timesheetRepository;
        public TimesheetService(ITimesheetRepository timesheetRepository)
        {
            _timesheetRepository = timesheetRepository;
        }
        public Task CreateTimesheetAsync(Timesheet timesheet)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTimesheetAsync(int timesheetId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Timesheet>> GenerateTimesheetAsync(ActivityRequirements[] requirements)
        {
            var generated = new List<Timesheet>();
            var mapper = new TimesheetGeneratorMapper();



            return generated;
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
