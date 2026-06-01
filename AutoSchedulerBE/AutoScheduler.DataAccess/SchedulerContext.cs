using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using AutoScheduler.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.DataAccess
{
    public class SchedulerContext : IdentityDbContext<User>
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
        public required DbSet<Availability> Availability { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Group>()
                .HasMany(g => g.Requirements)
                .WithMany(r => r.Groups)
                .UsingEntity<ActivityRequirementsGroup>();

            builder.Entity<ActivityRequirements>()
                .HasMany(r => r.Groups)
                .WithMany(g => g.Requirements)
                .UsingEntity<ActivityRequirementsGroup>();

            builder.Entity<ActivityRequirementsGroup>()
                .HasKey(arg => new { arg.GroupId, arg.ActivityRequirementsId });

            builder.Entity<ActivityRequirementsGroup>()
                .HasOne(arg => arg.Group)
                .WithMany(g => g.RequirementsGroups)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
