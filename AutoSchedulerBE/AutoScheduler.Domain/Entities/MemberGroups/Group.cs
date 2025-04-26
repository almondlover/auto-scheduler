using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.MemberGroups
{
    internal class Group
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? ParentGroupId { get; set; }
        public IList<Group>? SubGroups { get; set; }

    }
}
