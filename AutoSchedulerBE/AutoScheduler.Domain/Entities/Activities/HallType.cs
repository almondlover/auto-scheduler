using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    internal class HallType
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int Description { get; set; }

    }
}
