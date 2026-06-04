using AutoScheduler.Domain.DTOs.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Activities
{
    public class ActivityRequirementFromCsvDTO
    {
        public int Id { get; set; }
        public required string ActivityName { get; set; }
        public required string GroupNames { get; set; }
        public required string MemberName { get; set; }
        public int Duration { get; set; }
        public int? HallSize { get; set; }
        public string? HallTypeName { get; set; }
        public string? HallName { get; set; }

    }
}
