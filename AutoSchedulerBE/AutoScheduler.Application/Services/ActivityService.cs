using AutoMapper;
using AutoScheduler.Domain.DTOs.Activities;
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
        private readonly IMapper _mapper;
        public ActivityService(IActivityRepository activityRepository, IMapper mapper)
        {
            _activityRepository = activityRepository;
            _mapper = mapper;
        }
        public async Task CreateActivityAsync(ActivityDTO activityDto)
        {
            var activity = _mapper.Map<Activity>(activityDto);
            await _activityRepository.CreateActivityAsync(activity);
        }

        public async Task CreateActivityRequirementsAsync(ActivityRequirementsDTO requirementsDto)
        {
            var requirements = _mapper.Map<ActivityRequirements>(requirementsDto);
            await _activityRepository.CreateActivityRequirementsAsync(requirements);
        }

        public async Task CreateHallAsync(HallDTO hallDto)
        {
            var hall = _mapper.Map<Hall>(hallDto);
            await _activityRepository.CreateHallAsync(hall);
        }

        public async Task DeleteActivityAsync(int activityId)
        {
            await _activityRepository.DeleteActivityAsync(activityId);
        }

        public Task<IList<Activity>> GetActivitiesByMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<ActivityDTO>> GetActivitiesByOrganizationIdAsync(int organizationId)
        {
            return _mapper.Map<IList<ActivityDTO>>(await _activityRepository.GetActivitiesByOrganizationIdAsync(organizationId));
        }

        public async Task<ActivityDTO> GetActivityByIdAsync(int activityId)
        {
            return _mapper.Map<ActivityDTO>(await _activityRepository.GetActivityByIdAsync(activityId));
        }

        public async Task<IList<HallTypeDTO>> GetAllHallTypesAsync()
        {
            return _mapper.Map<IList<HallTypeDTO>>(await _activityRepository.GetAllHallTypesAsync());
        }

        public async Task<IList<ActivityRequirementsDTO>> GetRequirementsByGroupIdAsync(int groupId)
        {
            return _mapper.Map<IList<ActivityRequirementsDTO>>(await _activityRepository.GetRequirementsByGroupIdAsync(groupId));
        }

        public async Task UpdateActivityAsync(ActivityDTO activityDto)
        {
            var activity = _mapper.Map<Activity>(activityDto);
            await _activityRepository.UpdateActivityAsync(activity);
        }
    }
}
