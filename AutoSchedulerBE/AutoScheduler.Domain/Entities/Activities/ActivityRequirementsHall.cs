using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    public class ActivityRequirementsHall
    {
        public int HallId { get; set; }
        public Hall? Hall { get; set; }
        public int ActivityRequirementsId { get; set; }
        public ActivityRequirements? Requirements { get; set; }
    }
}
