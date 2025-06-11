using AutoScheduler.Domain.Entities.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Interfaces.Service
{
    public interface IActivityService
    {
        public Task<Activity> GetActivityByIdAsync(int activityId);
        public Task<IList<Activity>> GetActivitiesByMemberIdAsync(int memberId);
        public Task<IList<Activity>> GetActivitiesByOrganizationIdAsync(int organizationId);
        public Task<IList<ActivityRequirements>> GetRequirementsByGroupIdAsync(int groupId);
        public Task CreateActivityAsync(Activity activity);
        public Task UpdateActivityAsync(Activity activity);
        public Task DeleteActivityAsync(int activityId);
        public Task CreateActivityRequirementsAsync(ActivityRequirements requirements);
    }
}
