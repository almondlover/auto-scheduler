using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Activities
{
    public class HallTypeDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int Description { get; set; }
    }
}
