using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;

namespace AutoScheduler.Application.Utils
{
    public class GeneratorSlotProps
    {
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; }
        public int? GroupId { get; set; }
        public int MemberId { get; set; }
        public Member? Member { get; set; }
        public int Duration { get; set; }
    }
}
