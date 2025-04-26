using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Application.Services
{
    internal class ActivityService : IActivityService
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
