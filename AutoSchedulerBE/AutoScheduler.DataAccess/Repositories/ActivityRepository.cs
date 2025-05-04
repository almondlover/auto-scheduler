using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.DataAccess.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly SchedulerContext _dbContext;
        public ActivityRepository(SchedulerContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateActivityAsync(Activity activity)
        {
            try
            {
                await _dbContext.Activities.AddAsync(activity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save this activity");
            }
        }

        public async Task CreateActivityRequirementsAsync(ActivityRequirements requirements)
        {
            try
            {
                await _dbContext.ActivityRequirements.AddAsync(requirements);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save this activity requirement");
            }
        }

        public async Task DeleteActivityAsync(int activityId)
        {
            try
            {
                var activity = await _dbContext.Activities.Where(activity=>activity.Id==activityId).FirstOrDefaultAsync();
                
                _dbContext.Activities.Remove(activity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't delete this activity");
            };
        }

        public Task<IList<Activity>> GetActivitiesByMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Activity>> GetActivitiesByOrganizationIdAsync(int organizationId)
        {
            try
            {
                var activities = await _dbContext.Activities
                                                    .Where(activity => activity.OrganizationId == organizationId)
                                                    .ToListAsync();
                return activities;
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find these activities");
            }
        }

        public async Task<Activity> GetActivityByIdAsync(int activityId)
        {
            try
            {
                var activities = await _dbContext.Activities
                                                    .Where(activity => activity.Id == activityId)
                                                    .FirstOrDefaultAsync();
                return activities;
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find this activities");
            }
        }

        public Task<IList<ActivityRequirements>> GetRequirementsByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            try
            {
                _dbContext.Activities.Update(activity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't update this activity");
            }
        }
    }
}
