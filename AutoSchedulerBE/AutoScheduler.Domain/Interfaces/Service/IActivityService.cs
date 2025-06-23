using AutoScheduler.Domain.DTOs.Activities;
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
        public Task<ActivityDTO> GetActivityByIdAsync(int activityId);
        public Task<IList<Activity>> GetActivitiesByMemberIdAsync(int memberId);
        public Task<IList<ActivityDTO>> GetActivitiesByOrganizationIdAsync(int organizationId);
        public Task<IList<ActivityRequirementsDTO>> GetRequirementsByGroupIdAsync(int groupId);
        public Task<IList<HallTypeDTO>> GetAllHallTypesAsync();
        public Task CreateActivityAsync(ActivityDTO activityDto);
        public Task UpdateActivityAsync(ActivityDTO activityDto);
        public Task DeleteActivityAsync(int activityId);
        public Task CreateActivityRequirementsAsync(ActivityRequirementsDTO requirementsDto);
        public Task CreateHallAsync(HallDTO hallDto);
        public Task DeleteHallAsync(int hallId);
        public Task UpdateHallAsync(HallDTO hallDto);
    }
}
