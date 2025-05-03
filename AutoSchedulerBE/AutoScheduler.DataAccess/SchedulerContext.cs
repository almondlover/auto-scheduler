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
        public SchedulerContext(DbContextOptions<SchedulerContext> options) : base(options) { }
        public required DbSet<Timesheet> Timesheets { get; set; }
        public required DbSet<Timeslot> Timeslots { get; set; }
        public required DbSet<Group> Groups { get; set; }
        public required DbSet<Member> Members { get; set; }
        public required DbSet<Organization> Organizations { get; set; }
        public required DbSet<Activity> Activities { get; set; }
        public required DbSet<ActivityRequirements> ActivityRequirements { get; set; }
        public required DbSet<Hall> Halls { get; set; }
        public required DbSet<HallType> HallTypes { get; set; }
    }
}
