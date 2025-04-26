using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    internal class Activity
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int Description { get; set; }
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }
}
