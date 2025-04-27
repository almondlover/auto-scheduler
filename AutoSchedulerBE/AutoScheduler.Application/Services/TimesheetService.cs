using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Application.Services
{
    internal class TimesheetService : ITimesheetService
    {
        public Task CreateTimesheetAsync(Timesheet timesheet)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTimesheetAsync(int timesheetId)
        {
            throw new NotImplementedException();
        }

        public Task GenerateTimesheetAsync()
        {
            throw new NotImplementedException();
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
