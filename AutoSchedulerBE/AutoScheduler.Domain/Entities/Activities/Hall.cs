using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    public class Hall
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public required string Name { get; set; }
        public int Size { get; set; }
        public int HallTypeId { get; set; }
        public HallType? Type { get; set; }
    }
}
