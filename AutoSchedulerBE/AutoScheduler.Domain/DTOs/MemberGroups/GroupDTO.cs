using AutoScheduler.Domain.DTOs.Activities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.MemberGroups
{
    public class GroupDTO
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? ParentGroupId { get; set; }
        public IList<GroupDTO>? SubGroups { get; set; }
        public IList<ActivityRequirementsDTO>? Requirements { get; set; }
    }
}
