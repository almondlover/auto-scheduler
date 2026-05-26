using AutoScheduler.Domain.Entities.Activities;

namespace AutoScheduler.Domain.Extensions
{
    public static class ActivityTypeExtensions
    {
        public static ActivityType RootType(this ActivityType activityType)
        {
            if (activityType.BaseType == null) return activityType;

            return activityType.BaseType.RootType();
        }
    }
}
