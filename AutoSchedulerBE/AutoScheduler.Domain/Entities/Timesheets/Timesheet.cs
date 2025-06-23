using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduler.Domain.Entities.Timesheets
{
    public class Timesheet
	{
		public int Id { get; set; }
        public required string Title { get; set; }
        public bool Active { get; set; }
        public bool Optimized { get; set; }
        public IList<Timeslot>? Timeslots { get; set; }
    }
}
