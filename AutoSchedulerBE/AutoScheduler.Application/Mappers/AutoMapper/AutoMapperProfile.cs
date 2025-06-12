using AutoMapper;
using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.DTOs.MemberGroups;
using AutoScheduler.Domain.DTOs.Timesheets;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Application.Mappers.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //activities
            CreateMap<ActivityDTO, Activity>();
            CreateMap<Activity, ActivityDTO>();

            CreateMap<ActivityRequirementsDTO, ActivityRequirements>();
            CreateMap<ActivityRequirements, ActivityRequirementsDTO>();

            CreateMap<HallDTO, Hall>();
            CreateMap<Hall, HallDTO>();

            CreateMap<HallTypeDTO, HallType>();
            CreateMap<HallType, HallTypeDTO>();

            //groups
            CreateMap<GroupDTO, Group>();
            CreateMap<Group, GroupDTO>();

            CreateMap<MemberDTO, Member>();
            CreateMap<Member, MemberDTO>();

            CreateMap<OrganizationDTO, Organization>();
            CreateMap<Organization, OrganizationDTO>();

            CreateMap<AvailabilityDTO, Availability>();
            CreateMap<Availability, AvailabilityDTO>();

            //timesheets
            CreateMap<TimesheetDTO, Timesheet>();
            CreateMap<Timesheet, TimesheetDTO>();

            CreateMap<TimeslotDTO, Timeslot>();
            CreateMap<Timeslot, TimeslotDTO>();
        }
    }
}
