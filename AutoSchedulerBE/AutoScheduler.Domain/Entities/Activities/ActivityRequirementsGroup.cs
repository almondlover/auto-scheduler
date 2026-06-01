using AutoScheduler.Domain.Entities.MemberGroups;

namespace AutoScheduler.Domain.Entities.Activities
{
    public class ActivityRequirementsGroup
    {
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public int ActivityRequirementsId { get; set; }
        public ActivityRequirements? Requirements { get; set; }
    }
}
