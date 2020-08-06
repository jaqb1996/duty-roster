﻿using System;
using System.Runtime.Remoting.Messaging;

namespace ClassLibrary.Models
{
    public class WorkingOptionModel : IWorkingOption
    {
        public static string StartingHourFormat = "HH:mm"; // HH - 24h format
        public static string WorkingTimeFormat = @"hh\:mm";
        public int Id { get; set; }
        public string Symbol { get; set; }
        public TimeSpan WorkingTime { get; set; }
        public DateTime StartingHour { get; set; }

        public string GetSummary => $"{Symbol}, {StartingHour.ToString(StartingHourFormat)}, {WorkingTime.ToString(WorkingTimeFormat)}";

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