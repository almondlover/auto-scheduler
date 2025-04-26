using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.MemberGroups
{
    internal class Organization
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public IList<Group>? Groups { get; set; }
    }
}
