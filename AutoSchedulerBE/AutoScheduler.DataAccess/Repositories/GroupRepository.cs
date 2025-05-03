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
        public Task CreateGroupAsync(Group group)
        {
            throw new NotImplementedException();
        }

        public Task CreateOrganizationAsync(Organization organization)
        {
            throw new NotImplementedException();
        }

        public Task DeleteGroupAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOrganizationAsync(int organizationId)
        {
            throw new NotImplementedException();
        }

        public Task<Group> GetGroupByIdAsync(int groupId)
        {
            throw new NotImplementedException();
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

        public Task<Organization> GetOrganizationByIdAsync(int organizationId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateGroupAsync(Group group)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrganizationAsync(Organization organization)
        {
            throw new NotImplementedException();
        }
    }
}
