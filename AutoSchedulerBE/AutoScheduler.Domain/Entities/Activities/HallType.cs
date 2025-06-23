using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    public class HallType
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }

    }
}
