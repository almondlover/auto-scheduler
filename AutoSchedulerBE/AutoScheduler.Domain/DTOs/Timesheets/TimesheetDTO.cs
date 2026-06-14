using AutoScheduler.Domain.Enums;

namespace AutoScheduler.Domain.DTOs.Timesheets
{
    public class TimesheetDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public TimesheetState State { get; set; }
        public bool Optimized { get; set; }
        public int BaseSlotDuration { get; set; }
        public IList<TimeslotDTO>? Timeslots { get; set; }
    }
}
