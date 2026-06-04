using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AutoScheduler.DataAccess.Seeders
{
    public class RequirementSeeder
    {
        //seed full list of activity requirements and corresponding entities from csv
        public async static Task SeedFromCsvAsync(SchedulerContext dbContext)
        {
            var fullpath = Path.GetFullPath("../../");

            var filenames = Directory.GetFiles("../../", "*.Requirements.csv");

           if (filenames.IsNullOrEmpty())
                return;

            var filename = filenames[0].Replace("../../", "");

            string organizationName = filename.Split('.')[0];
            var orgId = (await dbContext.Organizations.FirstOrDefaultAsync(o => o.Name == organizationName))?.Id;

            if (orgId == null)
            {
                var newOrg = new Organization
                {
                    Name = organizationName,
                    Description = "Auto-generated organization"
                };
                dbContext.Organizations.Add(newOrg);
                await dbContext.SaveChangesAsync();
                orgId = (await dbContext.Organizations.FirstOrDefaultAsync(o => o.Name == organizationName))?.Id;
            }

            using (var csvReader = new CsvReader(
                    new StreamReader(
                            new FileStream(
                                    fullpath+filename,
                                    FileMode.Open,
                                    FileAccess.Read
                                )), System.Globalization.CultureInfo.CurrentCulture))
            {
                while (csvReader.Read())
                {
                    //read current record
                    var record = csvReader.GetRecord<ActivityRequirementFromCsvDTO>();

                    var activityId = await dbContext.Activities.Where(a => a.OrganizationId == orgId && a.Title == record.ActivityName).Select(a => a.Id).FirstOrDefaultAsync();
                    Activity? newActivity = null;
                    if (activityId == 0)
                    {
                        newActivity = new Activity
                        {
                            Title = record.ActivityName,
                            OrganizationId = orgId ?? 0,
                            Description = "Auto generated activity"
                        };
                    }

                    var memberId = await dbContext.Members.Where(m => m.OrganizationId == orgId && m.Name == record.MemberName).Select(m => m.Id).FirstOrDefaultAsync();
                    Member? newMember = null;
                    if (memberId == 0)
                    {
                        newMember = new Member
                        {
                            Name = record.MemberName,
                            OrganizationId = orgId ?? 0
                        };
                    }
                    //only works with max depth of 2 for group hierarchy in the current state
                    var parentGroupId = await dbContext.Groups.Where(g => g.OrganizationId == orgId && g.Name == record.MainGroupName).Select(g => g.Id).FirstOrDefaultAsync();
                    
                    if (parentGroupId == 0 && !record.MainGroupName.IsNullOrEmpty())
                    {
                        dbContext.Groups.Add(new Group
                        {
                            Name = record.MainGroupName!,
                            Description = "Auto generated group",
                            OrganizationId = orgId ?? 0
                        });
                        await dbContext.SaveChangesAsync();
                        parentGroupId = await dbContext.Groups.Where(g => g.OrganizationId == orgId && g.Name == record.MainGroupName).Select(g => g.Id).FirstOrDefaultAsync();
                    }

                    var groupNames = record.GroupNames.Split('/', StringSplitOptions.TrimEntries);

                    var groups = new List<Group>();
                    foreach (var groupName in groupNames)
                    {
                        var group = await dbContext.Groups.Where(g => g.OrganizationId == orgId && g.Name == groupName && (g.ParentGroupId == null || g.ParentGroupId == parentGroupId)).FirstOrDefaultAsync();
                        if (group == null)
                            groups.Add(new Group
                                {
                                    Name = groupName,
                                    Description = "Auto generated group",
                                    OrganizationId = orgId ?? 0,
                                    ParentGroupId = parentGroupId == 0 ? null : parentGroupId
                                });

                        else groups.Add(group);
                    }

                    var hallTypeId = await dbContext.HallTypes.Where(ht => ht.Title == record.HallTypeName).Select(ht => ht.Id).FirstOrDefaultAsync();
                    HallType? newHallType = null;
                    if (hallTypeId == 0 && !record.HallTypeName.IsNullOrEmpty())
                    {
                        newHallType = new HallType
                        {
                            Title = record.HallTypeName!,
                        };
                    }

                    //skip current record if there is already an identical requirement
                    //while in general such entries could exist, this list should only create unique requirements
                    //alternatively, also possible to set weekly repetition for activity and compare against that
                    //but that assumes further business logic changes
                    if ((await dbContext.ActivityRequirements
                        .Include(req => req.Groups)
                        .Where(req => req.ActivityId == activityId
                            && req.HallTypeId == hallTypeId
                            && req.MemberId == memberId
                            && req.Duration == record.Duration
                            && req.HallSize == record.HallSize).ToListAsync()).Any(req =>
                            !req.Groups.IsNullOrEmpty() && req.Groups!.Select(g => g.Id).Order().SequenceEqual(groups.Select(g => g.Id).Order()))
                        )
                        continue;

                    //map both ids (possibly 0) and entities (possibly null if id's available) and leave to ef to track changes
                    dbContext.ActivityRequirements.Add(new ActivityRequirements
                    {
                        ActivityId = activityId,
                        HallTypeId = hallTypeId,
                        MemberId = memberId,
                        Duration = record.Duration,
                        Groups = groups,
                        Activity = newActivity,
                        Member = newMember,
                        HallType = newHallType,
                        HallSize = record.HallSize ?? int.MaxValue
                    });
                    await dbContext.SaveChangesAsync();
                }
            }


        }
    }
}
