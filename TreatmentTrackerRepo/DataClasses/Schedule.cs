using System;

namespace TreatmentTrackerRepo.DataClasses
{
    public class Treatment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Frequency { get; set; }
        public int MaxPerDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public long StartTimeTicks { get; set; }

    }

    public class Schedule
    {
        public int Id { get; set; }
        public DateTime TreatmentTime { get; set; }
        public int TreatmentId { get; set; }
        public string TreatmentName { get; set; }
        public bool Administered { get; set; }
    }
}
