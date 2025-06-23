using AutoScheduler.Domain.DTOs.MemberGroups;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Activities
{
    public class HallDTO
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public required string Name { get; set; }
        public int Size { get; set; }
        public IList<AvailabilityDTO>? Availability { get; set; }
        public required HallTypeDTO Type { get; set; }
    }
}
