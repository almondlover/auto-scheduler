using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.DataAccess.Repositories
{
    internal class ActivityRepository : IActivityRepository
    {
        public Task CreateActivityAsync(Activity activity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteActivityAsync(int activityId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Activity>> GetActivitiesByMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Activity>> GetActivitiesByOrganizationIdAsync(int organizationId)
        {
            throw new NotImplementedException();
        }

        public Task<Activity> GetActivityByIdAsync(int activityId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ActivityRequirements>> GetRequirementsByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateActivityAsync(Activity activity)
        {
            throw new NotImplementedException();
        }
    }
}
