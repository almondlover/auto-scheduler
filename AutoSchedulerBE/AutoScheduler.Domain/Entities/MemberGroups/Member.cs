using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.MemberGroups
{
    internal class Member
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Contact { get; set; }
    }
}
