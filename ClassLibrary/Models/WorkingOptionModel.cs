using System;
using System.Runtime.Remoting.Messaging;

namespace ClassLibrary.Models
{
    public class WorkingOptionModel : IWorkingOption
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public TimeSpan WorkingTime { get; set; }
        public DateTime StartingHour { get; set; }

        public string GetSummary => $"{Symbol}, {StartingHour.ToString("hh:mm")}, {WorkingTime.ToString(@"hh\:mm")}";

        public WorkingOptionModel()
        {
            Symbol = "";
            WorkingTime = new TimeSpan();
            StartingHour = new DateTime();
        }
        public WorkingOptionModel(string symbol, TimeSpan workingTime, DateTime startingHour)
        {
            Symbol = symbol;
            WorkingTime = workingTime;
            StartingHour = startingHour;
        }
    }
}