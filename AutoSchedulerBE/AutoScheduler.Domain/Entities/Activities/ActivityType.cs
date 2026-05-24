using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Activities
{
    public class ActivityType
    {
        public int Id { get; set; }
        public int? BaseTypeId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        [ForeignKey("BaseTypeId")]
        public IList<ActivityType>? SubTypes { get; set; }
    }
}
