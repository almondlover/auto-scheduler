using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Application.Services
{
    internal class GroupService : IGroupService
    {
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

        public Task<IList<Group>> GetGroupsByOrganizationIdAsync(int organizationId)
        {
            throw new NotImplementedException();
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
