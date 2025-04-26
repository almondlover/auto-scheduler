using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    internal class ActivityRequirements
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public int Duration { get; set; }
        public int HallSize { get; set; }
        public string? HallType { get; set; }
        public int? TimesPerWeek { get; set; }
        public int? HallTypeId { get; set; }
        public HallType? Type { get; set; }
    }
}
