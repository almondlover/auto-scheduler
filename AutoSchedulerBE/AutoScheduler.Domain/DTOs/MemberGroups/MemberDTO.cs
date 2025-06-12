using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.MemberGroups
{
    public class MemberDTO
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public required string Name { get; set; }
        public string? Contact { get; set; }
        public IList<AvailabilityDTO>? Availability { get; set; }
    }
}
