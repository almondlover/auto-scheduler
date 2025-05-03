using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Interfaces.Repository;
using AutoScheduler.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        public GroupService (IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
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

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            return await _groupRepository.GetGroupByIdAsync(groupId);
        }

        public Task<IList<Group>> GetGroupsByMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Group>> GetGroupsByOrganizationIdAsync(int organizationId)
        {
            return await _groupRepository.GetGroupsByOrganizationIdAsync(organizationId);
        }

        public async Task<Organization> GetOrganizationByIdAsync(int organizationId)
        {
            return await _groupRepository.GetOrganizationByIdAsync(organizationId);
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
