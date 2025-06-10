using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.MemberGroups
{
    public class Member
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public required string Name { get; set; }
        public string? Contact { get; set; }
        public IList<Availability>? Availability { get; set; }
    }
}
