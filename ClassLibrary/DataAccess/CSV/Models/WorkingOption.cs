using ClassLibrary.Models;
using System;

namespace ClassLibrary.DataAccess.CSV.Models
{
    class WorkingOption : IWorkingOption
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime StartingHour { get; set; }
        public TimeSpan WorkingTime { get; set; }
    }
}
