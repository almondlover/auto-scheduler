using AutoMapper;
using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.DTOs.MemberGroups;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;

namespace AutoScheduler.Application.Mappers.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //activities
            CreateMap<ActivityDTO, Activity>()
                .ForMember(dest => dest.ActivityTypeId, opt => opt.MapFrom(src => src.Type.Id));
            CreateMap<Activity, ActivityDTO>();

            CreateMap<ActivityTypeDTO, ActivityType>();
            CreateMap<ActivityType, ActivityTypeDTO>()
                .ForMember(dest => dest.BaseTypeName, opt => opt.MapFrom(src => src.BaseType.Title));

            CreateMap<ActivityRequirementsDTO, ActivityRequirements>()
                .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.Member.Id))
                .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.Activity.Id))
                .ForMember(dest => dest.HallTypeId, opt => opt.MapFrom(src => src.HallType.Id));
            CreateMap<ActivityRequirements, ActivityRequirementsDTO>();

            CreateMap<HallDTO, Hall>()
                .ForMember(dest => dest.HallTypeId, opt => opt.MapFrom(src => src.Type.Id))
                .ForMember(dest => dest.Type, opt => opt.Ignore());
            CreateMap<Hall, HallDTO>();

            CreateMap<HallTypeDTO, HallType>();
            CreateMap<HallType, HallTypeDTO>();

            //groups
            CreateMap<GroupDTO, Group>();
            CreateMap<Group, GroupDTO>()
                .ForMember(dest => dest.Requirements, opt => opt.Ignore());

            CreateMap<MemberDTO, Member>();
            CreateMap<Member, MemberDTO>();

            CreateMap<OrganizationDTO, Organization>();

            CreateMap<Organization, OrganizationDTO>();

            CreateMap<AvailabilityDTO, Availability>();
            CreateMap<Availability, AvailabilityDTO>();

            //timesheets
            CreateMap<TimesheetDTO, Timesheet>();
            CreateMap<Timesheet, TimesheetDTO>();

            CreateMap<TimeslotDTO, Timeslot>()
                .ForMember(dest => dest.HallId, opt => opt.MapFrom(src => src.Hall.Id))
                .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.Member.Id))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Group.Id))
                .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.Activity.Id))
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.Activity, opt => opt.Ignore())
                .ForMember(dest => dest.Member, opt => opt.Ignore())
                .ForMember(dest => dest.Hall, opt => opt.Ignore());
            CreateMap<Timeslot, TimeslotDTO>();
        }
    }
}
