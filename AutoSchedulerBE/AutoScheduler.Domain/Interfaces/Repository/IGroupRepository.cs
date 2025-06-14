using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Interfaces.Repository
{
    public interface IGroupRepository
    {
        public Task<Group> GetGroupByIdAsync(int groupId);
        public Task<Organization> GetOrganizationByIdAsync(int organizationId);
        public Task<IList<Organization>> GetAllOrganizationsAsync();
        public Task<IList<Group>> GetGroupsByOrganizationIdAsync(int organizationId);
        public Task<IList<Group>> GetGroupsByMemberIdAsync(int memberId);
        public Task CreateGroupAsync(Group group);
        public Task CreateOrganizationAsync(Organization organization);
        public Task UpdateGroupAsync(Group group);
        public Task UpdateOrganizationAsync(Organization organization);
        public Task DeleteGroupAsync(int groupId);
        public Task DeleteOrganizationAsync(int organizationId);
        public Task CreateMemberAsync(Member member);
    }
}
