using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.DataAccess
{
    public class SchedulerContext:DbContext
    {
        public required DbSet<Timesheet> Timesheets;
        public required DbSet<Timeslot> Timeslots;
        public required DbSet<Group> Groups;
        public required DbSet<Member> Members;
        public required DbSet<Organization> Organizations;
        public required DbSet<Activity> Activities;
        public required DbSet<ActivityRequirements> ActivityRequirements;
        public required DbSet<Hall> Halls;
        public required DbSet<HallType> HallTypes;
    }
}
