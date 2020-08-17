using RosterLibrary.Models;
using System;

namespace RosterLibrary.DataAccess.CSV.Models
{
    class Schedule : ISchedulePresentationData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartingDay { get; set; }
        public DateTime LastDay { get; set; }

    }
}
