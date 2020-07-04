using ClassLibrary.Calculations.Helpers;
using ClassLibrary.Helpers;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Calculations
{
    public class HolidayNightHoursCalculator : ICalculatorReturningNumberForEmployee
    {
        const double HoursFromNotHolidayToHoliday = 1;
        const double HoursForHoliday = 4;
        public string Name => "Kalkulator godzin nocnych świątecznych";
        public string Description => "Kalkulator obliczający liczbę godzin nocnych świątecznych dla każdego pracownika";
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
                        if (!HolidayChecker.isHoliday(date) && HolidayChecker.isHoliday(date.AddDays(1)))
                        {
                            numberOfHours += HoursFromNotHolidayToHoliday;
                        }
                        // This is a holiday
                        else if (HolidayChecker.isHoliday(date))
                        {
                            numberOfHours += HoursForHoliday;
                        }
                    }
                    output.Add(employee, numberOfHours);
                });
            }
            return output;
        }
    }
}
