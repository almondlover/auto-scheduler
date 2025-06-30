using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Interfaces.Repository
{
    public interface ITimesheetRepository
    {
        public Task<Timesheet> GetTimesheetByIdAsync(int timesheetId);
        public Task<Timesheet> GetTimesheetByGroupIdAsync(int groupId);
        public Task<IList<Timesheet>> GetTimesheetsForMemberAsync(int memberId);
        public Task<IList<Timeslot>> GetTimeslotsForMemberAsync(int memberId);
        public Task<IList<Timesheet>> GetOptimizedTimesheetAsync(int timesheetId);
        public Task GenerateTimesheetAsync();
        public Task CreateTimesheetAsync(Timesheet timesheet);
        public Task CreateAvailabilityRangeAsync(IList<Availability> availabilityList);
        public Task UpdateTimesheetAsync(Timesheet timesheet);
        public Task DeleteTimesheetAsync(int timesheetId);
        public Task<IList<Hall[]>> GetHallsForRequirementsAsync (ActivityRequirements[] requirements);
        public Task<IList<Group>> GetGroupsForRequirementsAsync(ActivityRequirements[] requirements);
    }
}
