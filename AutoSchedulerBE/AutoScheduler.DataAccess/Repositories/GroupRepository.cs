using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.DataAccess.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SchedulerContext _dbContext;
        public GroupRepository(SchedulerContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateGroupAsync(Group group)
        {
            try
            {
                await _dbContext.Groups.AddAsync(group);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save this group");
            }
        }

        public async Task CreateOrganizationAsync(Organization organization)
        {
            try
            {
                await _dbContext.Organizations.AddAsync(organization);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save this organization");
            }
        }

        public Task DeleteGroupAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteOrganizationAsync(int organizationId)
        {
            try
            {
                var organization = await _dbContext.Organizations.Where(org => org.Id == organizationId).FirstOrDefaultAsync();

                _dbContext.Organizations.Remove(organization);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't delete this organization");
            }
            ;
        }

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            try
            {
                return await _dbContext.Groups
                                        .Where(group => group.Id == groupId)
                                        .Include(group => group.Requirements)
                                        .Include(group => group.SubGroups)
                                        .FirstOrDefaultAsync();
            }
            catch (DbException exception)
            {
                throw new Exception($"Couldn't find this group: {exception}");
            }
        }

        public Task<IList<Group>> GetGroupsByMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Group>> GetGroupsByOrganizationIdAsync(int organizationId)
        {
            try
            {
                return await _dbContext.Groups
                                        .Where(group => group.OrganizationId == organizationId)
                                        .Include(group => group.Requirements)
                                        .Include(group => group.SubGroups)
                                        .ToListAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find the groups for this organization");
            }
        }

        public async Task<Organization> GetOrganizationByIdAsync(int organizationId)
        {
            try
            {
                return await _dbContext.Organizations
                                        .Where(org => org.Id == organizationId)
                                        .Include(org => org.Groups)
                                        .Include(org => org.Members)
                                            .ThenInclude(member=>member.Availability)
                                        .Include(org => org.Halls)
                                            .ThenInclude(hall => hall.Availability)
                                        .Include(org => org.Activities)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find this organization");
            }
        }

        public async Task UpdateGroupAsync(Group group)
        {
            try
            {
                _dbContext.Groups.Update(group);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't update this group");
            }
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            try
            {
                _dbContext.Organizations.Update(organization);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't update this organization");
            }
        }
    }
}
