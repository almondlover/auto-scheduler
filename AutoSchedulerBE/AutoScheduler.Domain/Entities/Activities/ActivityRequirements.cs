using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    public class ActivityRequirements
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; }
        public int? GroupId { get; set; }
        public int MemberId { get; set; }
        public Member? Member { get; set; }
        public int Duration { get; set; }
        public int HallSize { get; set; }
        public int? TimesPerWeek { get; set; }
        public int? HallTypeId { get; set; }
        public HallType? HallType { get; set; }
    }
}
