using AutoScheduler.Domain.Entities.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.MemberGroups
{
    public class Organization
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public IList<Group>? Groups { get; set; }
        public IList<Hall>? Halls { get; set; }
        public IList<Activity>? Activities { get; set; }
        public IList<Member>? Members { get; set; }
    }
}
