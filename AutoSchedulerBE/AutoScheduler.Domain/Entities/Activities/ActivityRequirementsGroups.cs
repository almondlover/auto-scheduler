using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    public class ActivityRequirementsGroup
    {
        public int GroupId { get; set; }
        public int ActivityRequirementsId { get; set; }
    }
}
