using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace AutoScheduler.DataAccess.Repositories
{
	public class TimesheetRepository : ITimesheetRepository
	{
		private readonly SchedulerContext _dbContext;
		public TimesheetRepository(SchedulerContext dbContext)
		{
			_dbContext = dbContext;
		}
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

        public async Task<IList<Group>> GetGroupsForRequirementsAsync(ActivityRequirements[] requirements)
        {
            try
            {
                var typeIndexes = requirements.Select(req => req.HallTypeId);

                return await _dbContext.Groups
                                        .Where(group => requirements
                                            .Select(req => req.GroupId)
                                            .Contains(group.Id))
                                        .ToListAsync();
            }
            catch (DbException exception)
            {
                throw new Exception($"Couldn't find this group: {exception}");
            }
        }

        public async Task<IList<Hall[]>> GetHallsForRequirementsAsync(ActivityRequirements[] requirements)
		{
			try
			{
				var result = new List<Hall[]>();
				//should look into how to query this instead
				foreach (var requirement in requirements)
                    result.Add( await _dbContext.Halls
										.Where(hall => hall.HallTypeId == requirement.HallTypeId)
											.Include(hall => hall.Availability)
										.AsNoTracking()
                                        .ToArrayAsync());
				return result;
			}
			catch (DbException exception)
			{
				throw new Exception($"Couldn't find the halls per the requirements: {exception}");
			}
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
