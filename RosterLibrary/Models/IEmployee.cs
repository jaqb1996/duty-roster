﻿using System;
using System.Collections.Generic;

namespace RosterLibrary.Models
{
    public interface IEmployee : IEmployeePresentationData
    {
        List<IWorkingOption> WorkingPlan { get; }
        List<IWorkingOption> AvailableOptions { get; }
        void ChangeWorkDay(int indexOfDate, string symbol, DateTime startingHour, TimeSpan workingTime);
    }
}