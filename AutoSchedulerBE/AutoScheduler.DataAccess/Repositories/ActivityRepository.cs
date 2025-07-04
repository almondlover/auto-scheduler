﻿using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoScheduler.DataAccess.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly SchedulerContext _dbContext;
        public ActivityRepository(SchedulerContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateActivityAsync(Activity activity)
        {
            try
            {
                await _dbContext.Activities.AddAsync(activity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save this activity");
            }
        }

        public async Task CreateActivityRequirementsAsync(ActivityRequirements requirements)
        {
            try
            {
                _dbContext.Attach(requirements.Activity);
                _dbContext.Attach(requirements.Member);
                _dbContext.Attach(requirements.HallType);
                await _dbContext.ActivityRequirements.AddAsync(requirements);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save this activity requirement");
            }
        }

        public async Task CreateHallAsync(Hall hall)
        {
            try
            {
                await _dbContext.Halls.AddAsync(hall);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception($"Couldn't save this hall: {exception.Message}");
            }
        }

        public async Task DeleteActivityAsync(int activityId)
        {
            try
            {
                var activity = await _dbContext.Activities.Where(activity=>activity.Id==activityId).FirstOrDefaultAsync();
                
                _dbContext.Activities.Remove(activity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't delete this activity");
            };
        }

        public async Task DeleteHallAsync(int hallId)
        {
            try
            {
                var hall = await _dbContext.Halls.Where(hall => hall.Id == hallId).FirstOrDefaultAsync();

                _dbContext.Halls.Remove(hall);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't delete this hall");
            }
            ;
        }

        public Task<IList<Activity>> GetActivitiesByMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Activity>> GetActivitiesByOrganizationIdAsync(int organizationId)
        {
            try
            {
                var activities = await _dbContext.Activities
                                                    .Where(activity => activity.OrganizationId == organizationId)
                                                    .AsNoTracking()
                                                    .ToListAsync();
                return activities;
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find these activities");
            }
        }

        public async Task<Activity> GetActivityByIdAsync(int activityId)
        {
            try
            {
                var activities = await _dbContext.Activities
                                                    .Where(activity => activity.Id == activityId)
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync();
                return activities;
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find this activities");
            }
        }

        public async Task<IList<HallType>> GetAllHallTypesAsync()
        {
            try
            {
                var types = await _dbContext.HallTypes
                                            .AsNoTracking()
                                            .ToListAsync();
                return types;
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find hall types: " + exception.Message);
            }
        }

        public async Task<IList<ActivityRequirements>> GetRequirementsByGroupIdAsync(int groupId)
        {
            try
            {
                var requirements = await _dbContext.ActivityRequirements
                                                    .Where(requirement => requirement.GroupId == groupId)
                                                    .Include(req => req.Activity)
                                                    .Include(req => req.Member)
                                                        .ThenInclude(member=>member.Availability)
                                                    .Include(req => req.HallType)
                                                    .AsNoTracking()
                                                    .ToListAsync();
                return requirements;
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find these requirements: " + exception.Message);
            }
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            try
            {
                _dbContext.Activities.Update(activity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't update this activity");
            }
        }

        public async Task UpdateHallAsync(Hall hall)
        {
            try
            {
                _dbContext.Halls.Update(hall);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't update this hall");
            }
        }
    }
}
