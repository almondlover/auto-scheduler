using AutoScheduler.Domain.DTOs.MemberGroups;
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

        public async Task CreateMemberAsync(Member member)
        {
            try
            {
                await _dbContext.Members.AddAsync(member);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't save this member");
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

        public async Task DeleteAvailabilityAsync(int availabilityId)
        {
            try
            {
                var availability = await _dbContext.Availability.Where(avail => avail.Id == availabilityId).FirstOrDefaultAsync();

                _dbContext.Availability.Remove(availability);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't delete occupation");
            };
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            try
            {
                var group = await _dbContext.Groups.Where(grp => grp.Id == groupId).FirstOrDefaultAsync();

                _dbContext.Groups.Remove(group);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't delete this group");
            };
        }

        public async Task DeleteMemberAsync(int memberId)
        {
            try
            {
                var member = await _dbContext.Members.Where(mem => mem.Id == memberId).FirstOrDefaultAsync();

                _dbContext.Members.Remove(member);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't delete this group");
            }
            ;
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
            };
        }

        public async Task<IList<Organization>> GetAllOrganizationsAsync()
        {
            try
            {
                return await _dbContext.Organizations
                                        .Include(org => org.Groups)
                                        .Include(org => org.Members)
                                            .ThenInclude(member => member.Availability)
                                        .Include(org => org.Halls)
                                            .ThenInclude(hall => hall.Availability)
                                        .Include(org => org.Halls)
                                            .ThenInclude(hall => hall.Type)
                                        .Include(org => org.Activities)
                                        .AsNoTracking()
                                        .ToListAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find organizations");
            }
        }

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            try
            {
                return await _dbContext.Groups
                                        .Where(group => group.Id == groupId)
                                        .Include(group => group.Requirements)
                                        .Include(group => group.SubGroups)
                                        .AsNoTracking()
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
                                            .ThenInclude(req=>req.HallType)
                                        .Include(group => group.SubGroups)
                                        .AsNoTracking()
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
                                         .Include(org => org.Halls)
                                            .ThenInclude(hall => hall.Type)
                                        .Include(org => org.Activities)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find this organization");
            }
        }

        public async Task<IList<Group>> GetRootGroupsByOrganizationIdAsync(int organizationId)
        {
            try
            {
                return await _dbContext.Groups
                                        .Where(group => group.OrganizationId == organizationId 
                                            && group.ParentGroupId==null)
                                        .Include(group => group.Requirements)
                                            .ThenInclude(req => req.HallType)
                                        .Include(group => group.SubGroups)
                                        .AsNoTracking()
                                        .ToListAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't find the root groups for this organization: "+exception.Message);
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

        public async Task UpdateMemberAsync(Member member)
        {
            try
            {
                _dbContext.Members.Update(member);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbException exception)
            {
                throw new Exception("Couldn't update this member");
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
