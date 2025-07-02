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

        public async Task CreateAvailabilityRangeAsync(IList<Availability> availabilityList)
        {
            try
            {
                await _dbContext.Availability.AddRangeAsync(availabilityList);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save availability");
            }
        }

        public async Task CreateTimesheetAsync(Timesheet timesheet)
		{
            try
            {
                await _dbContext.Timesheets.AddAsync(timesheet);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save this timesheet");
            }
        }

		public async Task DeleteTimesheetAsync(int timesheetId)
		{
            //only deactivates in order to keep this timesheet as history entry
            //should probably add seperate method for this and instead do a permanent delete as well
            try
            {
                var timesheet = await _dbContext.Timesheets.Where(ts => ts.Id == timesheetId).FirstOrDefaultAsync();

                //maybe eventually delete all timeslots and convert to json string to save history as suggested
                timesheet.Active = false;
                _dbContext.Timesheets.Update(timesheet);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't deactivate timesheet");
            }
            ;
        }

        public async Task DeleteTimeslotsAvailability(IList<Timeslot> timeslots)
        {
            try
            {
                var result = new List<Availability>();
                //should look into how to query this instead
                foreach (var timeslot in timeslots)
                    result.Add(await _dbContext.Availability
                                        .Where(avail => avail.HallId == timeslot.HallId
                                            && avail.MemberId >= timeslot.MemberId
                                            && avail.StartTime == timeslot.StartTime
                                            && avail.EndTime == timeslot.EndTime
                                            && avail.DayOfTheWeek == timeslot.DayOfWeek)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync());

                _dbContext.RemoveRange(result);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception($"Couldn't remove member/hall availability from timesheet: {exception}");
            }
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
										.AsNoTracking()
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
										.Where(hall => hall.HallTypeId == requirement.HallTypeId
											&& hall.Size>=requirement.HallSize)
											.Include(hall => hall.Availability)
                                            .Include(hall => hall.Type)
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

		public async Task<Timesheet> GetTimesheetByIdAsync(int timesheetId)
		{
            try
            {
                return await _dbContext.Timesheets
                                        .Where(timesheet => timesheet.Id==timesheetId)
                                        .Include(timesheet=>timesheet.Timeslots)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();
            }
            catch (DbException exception)
            {
                throw new Exception($"Couldn't find this group: {exception}");
            }
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
