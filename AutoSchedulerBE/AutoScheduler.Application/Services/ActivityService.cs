using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Interfaces.Repository;
using AutoScheduler.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        public ActivityService(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }
        public async Task CreateActivityAsync(Activity activity)
        {
            await _activityRepository.CreateActivityAsync(activity);
        }

        public async Task CreateActivityRequirementsAsync(ActivityRequirements requirements)
        {
            await _activityRepository.CreateActivityRequirementsAsync(requirements);
        }

        public async Task DeleteActivityAsync(int activityId)
        {
            await _activityRepository.DeleteActivityAsync(activityId);
        }

        public Task<IList<Activity>> GetActivitiesByMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Activity>> GetActivitiesByOrganizationIdAsync(int organizationId)
        {
            return await _activityRepository.GetActivitiesByOrganizationIdAsync(organizationId);
        }

        public async Task<Activity> GetActivityByIdAsync(int activityId)
        {
            return await _activityRepository.GetActivityByIdAsync(activityId);
        }

        public Task<IList<ActivityRequirements>> GetRequirementsByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            await _activityRepository.UpdateActivityAsync(activity);
        }
    }
}
