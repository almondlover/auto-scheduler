using AutoMapper;
using AutoScheduler.Domain.DTOs.MemberGroups;
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
        private readonly IMapper _mapper;
        public GroupService (IGroupRepository groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }
        public async Task CreateGroupAsync(GroupDTO groupDto)
        {
            var group = _mapper.Map<Group>(groupDto);
            await _groupRepository.CreateGroupAsync(group);
        }

        public async Task CreateMemberAsync(MemberDTO memberDto)
        {
            var member = _mapper.Map<Member>(memberDto);
            await _groupRepository.CreateMemberAsync(member);
        }

        public async Task CreateOrganizationAsync(OrganizationDTO organizationDto)
        {
            var organization = _mapper.Map<Organization>(organizationDto);
            await _groupRepository.CreateOrganizationAsync(organization);
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            await _groupRepository.DeleteGroupAsync(groupId);
        }

        public async Task DeleteMemberAsync(int memberId)
        {
            await _groupRepository.DeleteMemberAsync(memberId);
        }

        public async Task DeleteOrganizationAsync(int organizationId)
        {
            await _groupRepository.DeleteOrganizationAsync(organizationId);
        }

        public async Task<IList<OrganizationDTO>> GetAllOrganizationsAsync()
        {
            return _mapper.Map<IList<OrganizationDTO>>(await _groupRepository.GetAllOrganizationsAsync());
        }

        public async Task<GroupDTO> GetGroupByIdAsync(int groupId)
        {
            return _mapper.Map<GroupDTO>(await _groupRepository.GetGroupByIdAsync(groupId));
        }

        public Task<IList<Group>> GetGroupsByMemberIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<GroupDTO>> GetGroupsByOrganizationIdAsync(int organizationId)
        {
            return _mapper.Map<IList<GroupDTO>>(await _groupRepository.GetGroupsByOrganizationIdAsync(organizationId));
        }

        public async Task<OrganizationDTO> GetOrganizationByIdAsync(int organizationId)
        {
            return _mapper.Map<OrganizationDTO>(await _groupRepository.GetOrganizationByIdAsync(organizationId));
        }

        public async Task UpdateGroupAsync(GroupDTO groupDto)
        {
            var group = _mapper.Map<Group>(groupDto);
            await _groupRepository.UpdateGroupAsync(group);
        }

        public async Task UpdateOrganizationAsync(OrganizationDTO organizationDto)
        {
            var organization = _mapper.Map<Organization>(organizationDto);
            await _groupRepository.UpdateOrganizationAsync(organization);
        }
    }
}
