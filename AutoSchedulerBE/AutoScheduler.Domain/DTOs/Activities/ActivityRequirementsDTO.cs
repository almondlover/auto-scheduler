using AutoScheduler.Domain.DTOs.MemberGroups;
using AutoScheduler.Domain.Entities.MemberGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.DTOs.Activities
{
    public class ActivityRequirementsDTO
    {
        public int Id { get; set; }
        public ActivityDTO? Activity { get; set; }
        public GroupDTO? Group { get; set; }
        public MemberDTO? Member { get; set; }
        public int Duration { get; set; }
        //hall requirements nullable since an option could be added to only set hall
        public int? HallSize { get; set; }
        public HallTypeDTO? HallType { get; set; }
    }
}
