using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.MemberGroups
{
    public class OrganizationDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public IList<GroupDTO>? Groups { get; set; }
        public IList<HallDTO>? Halls { get; set; }
        public IList<ActivityDTO>? Activities { get; set; }
        public IList<MemberDTO>? Members { get; set; }
    }
}
