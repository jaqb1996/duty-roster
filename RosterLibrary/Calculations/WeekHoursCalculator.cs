﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RosterLibrary.Helpers;
using RosterLibrary.Models;

namespace RosterLibrary.Calculations
{
    public class WeekHoursCalculator : ICalculatorReturningListOfNumbers
    {
        public string Name { get; } = "Godziny pracy w tygodniu";
        public string Description { get; } = "Kalkulator obliczający godziny pracy w tygodniu dla każdego pracownika";

        public string ResultName => "Tydzień";

        public Dictionary<IEmployeePresentationData, List<double>> Calculate(ISchedule schedule)
        {
            Dictionary<IEmployeePresentationData, List<double>> output = new Dictionary<IEmployeePresentationData, List<double>>();  
            
            foreach (IEmployee employee in schedule.Employees)
            {
                List<double> workingTimes= new List<double>();
                double workingTimeForWeek = 0;
                int numberOfDays = employee.WorkingPlan.Count;
                for (int i = 0; i < numberOfDays; i++)
                {
                    IWorkingOption planForDay = employee.WorkingPlan[i];
                    workingTimeForWeek += planForDay.WorkingTime.TotalHours;
                    if (i % 7 == 6 || i == numberOfDays - 1)
                    {
                        workingTimes.Add(workingTimeForWeek);
                        workingTimeForWeek = 0;
                    }
                }
                output.Add(employee, workingTimes);
            }
            return output;
        }

    }
}
