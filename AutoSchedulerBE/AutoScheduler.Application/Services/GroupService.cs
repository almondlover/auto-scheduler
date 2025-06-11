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
        public async Task CreateGroupAsync(Group group)
        {
            await _groupRepository.CreateGroupAsync(group);
        }

        public async Task CreateOrganizationAsync(Organization organization)
        {
            await _groupRepository.CreateOrganizationAsync(organization);
        }

        public Task DeleteGroupAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteOrganizationAsync(int organizationId)
        {
            await _groupRepository.DeleteOrganizationAsync(organizationId);
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

        public async Task UpdateGroupAsync(Group group)
        {
            await _groupRepository.UpdateGroupAsync(group);
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            await _groupRepository.UpdateOrganizationAsync(organization);
        }
    }
}
