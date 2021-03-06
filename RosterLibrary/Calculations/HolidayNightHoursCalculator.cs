﻿using RosterLibrary.Calculations.Helpers;
using RosterLibrary.Helpers;
using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosterLibrary.Calculations
{
    public class HolidayNightHoursCalculator : ICalculatorReturningNumberForEmployee
    {
        const double HoursFoNightFromNotHolidayToHoliday = 1;
        const double HoursForHolidayNight = 4;
        public string Name => "Godziny nocne świąteczne";
        public string Description => "Kalkulator obliczający liczbę godzin nocnych świątecznych dla każdego pracownika. " +
                                     "Bazuje na nazwaniu nocnej zmiany symbolem N";

        public string ResultName => "Liczba godzin";

        public Dictionary<IEmployeePresentationData, double> Calculate(ISchedule schedule)
        {
            Dictionary<IEmployeePresentationData, double> output = new Dictionary<IEmployeePresentationData, double>();

            foreach (IEmployee employee in schedule.Employees)
            {
                int i = 0;
                double numberOfHours = 0;
                schedule.IterateOverAllDays((date) =>
                {
                    IWorkingOption planForDay = employee.WorkingPlan[i++];
                    // This is a night
                    if (planForDay.Symbol == "N")
                    {
                        // This is not a holiday but the next day is
                        if (!HolidayChecker.IsHoliday(date) && HolidayChecker.IsHoliday(date.AddDays(1)))
                        {
                            numberOfHours += HoursFoNightFromNotHolidayToHoliday;
                        }
                        // This is a holiday
                        else if (HolidayChecker.IsHoliday(date))
                        {
                            numberOfHours += HoursForHolidayNight;
                        }
                    }  
                });
                output.Add(employee, numberOfHours);
            }
            return output;
        }
    }
}
