using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Activities
{
    public class HallFromCsvDTO
    {
        public required string Name { get; set; }
        public required string HallTypeName { get; set; }
        public int HallSize { get; set; }
    }
}
