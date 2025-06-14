using AutoScheduler.Domain.DTOs.MemberGroups;
using AutoScheduler.Domain.Entities.MemberGroups;
using AutoScheduler.Domain.Entities.Timesheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Interfaces.Service
{
    public interface IGroupService
    {
        public Task<GroupDTO> GetGroupByIdAsync(int groupId);
        public Task<OrganizationDTO> GetOrganizationByIdAsync(int organizationId);
        public Task<IList<OrganizationDTO>> GetAllOrganizationsAsync();
        public Task<IList<GroupDTO>> GetGroupsByOrganizationIdAsync(int organizationId);
        public Task<IList<Group>> GetGroupsByMemberIdAsync(int memberId);
        public Task CreateGroupAsync(GroupDTO groupDto);
        public Task CreateOrganizationAsync(OrganizationDTO organizationDto);
        public Task UpdateGroupAsync(GroupDTO groupDto);
        public Task UpdateOrganizationAsync(OrganizationDTO organizationDto);
        public Task DeleteGroupAsync(int groupId);
        public Task DeleteOrganizationAsync(int organizationId);
        public Task CreateMemberAsync(MemberDTO memberDto);
        public Task DeleteMemberAsync(int memberId);
    }
}
