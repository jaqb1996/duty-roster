using System;

namespace RosterLibrary.Models
{
    public interface IWorkingOption
    {
        int Id { get; set; }
        DateTime StartingHour { get; set; }
        string Symbol { get; set; }
        TimeSpan WorkingTime { get; set; }
        string GetSummary { get; }
    }
}