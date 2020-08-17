using RosterLibrary.Models;
using System;

namespace RosterLibrary.DataAccess.CSV.Models
{
    class WorkingOption : IWorkingOption
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime StartingHour { get; set; }
        public TimeSpan WorkingTime { get; set; }

        public string GetSummary => $"{Symbol}, {StartingHour.ToString(WorkingOptionModel.StartingHourFormat)}, {WorkingTime.ToString(WorkingOptionModel.WorkingTimeFormat)}";
    }
}
