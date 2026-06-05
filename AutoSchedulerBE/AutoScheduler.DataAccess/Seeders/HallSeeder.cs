using AutoScheduler.Domain.DTOs.Activities;
using AutoScheduler.Domain.Entities.Activities;
using AutoScheduler.Domain.Entities.MemberGroups;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.DataAccess.Seeders
{
    public class HallSeeder
    {
        public async static Task SeedFromCsvAsync(SchedulerContext dbContext)
        {
            var fullpath = Path.GetFullPath(".");

            var filenames = Directory.GetFiles(".", "*.Halls.csv");

            if (filenames.IsNullOrEmpty())
                return;

            var filename = filenames[0].Replace(".", "");

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
                                    fullpath + filename,
                                    FileMode.Open,
                                    FileAccess.Read
                                )), System.Globalization.CultureInfo.CurrentCulture))
            {
                while (csvReader.Read())
                {
                    //read current record
                    var record = csvReader.GetRecord<HallFromCsvDTO>();

                    var hallTypeId = await dbContext.HallTypes.Where(ht => ht.Title == record.HallTypeName).Select(ht => ht.Id).FirstOrDefaultAsync();
                    HallType? newHallType = null;
                    if (hallTypeId == 0)
                    {
                        newHallType = new HallType
                        {
                            Title = record.HallTypeName,
                        };
                    }

                    var hallId = await dbContext.Halls.Where(h => h.OrganizationId == orgId && h.Name == record.Name).Select(h => h.Id).FirstOrDefaultAsync();
                    Hall? newHall = null;
                    if (hallId == 0)
                    {
                        newHall = new Hall
                        {
                            Name = record.Name,
                            OrganizationId = orgId ?? 0,
                            HallTypeId = hallTypeId,
                            Size = record.HallSize,
                            Type = newHallType
                        };
                    }

                    if (newHall == null)
                        continue;

                    dbContext.Halls.Add(newHall);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
