using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingApplication
{
    public static class ConsoleHelpers
    {
        const int MARGIN = 10;
        const string FORMAT = "dd.MM";
        //public static string FullSchedule(this ScheduleModel schedule)
        //{
        //    StringBuilder output = new StringBuilder();
        //    output.AppendLine(schedule.ColumnHeaders());
        //    foreach (var e in schedule.Employees)
        //    {
        //        output.AppendLine(e.GetWorkingPlanAsRow());
        //    }
        //    return output.ToString();
        //}
        private static string ColumnHeaders(this ScheduleModel schedule)
        {
            StringBuilder output = new StringBuilder();
            output.Append($"{"Pracownicy",MARGIN}");
            foreach (var day in schedule.WorkingDays)
            {
                output.Append($"{day.ToString(FORMAT),MARGIN}");
            }
            return output.ToString();
        }
        //public static string GetWorkingPlanAsRow(this IEmployee employee)
        //{
        //    StringBuilder output = new StringBuilder();
        //    output.Append($"{employee.FullName,MARGIN}");
        //    foreach (var workingOption in employee.WorkingPlan)
        //    {
        //        output.Append($"{workingOption.Symbol,MARGIN}");
        //    }
        //    return output.ToString();
        //}
    }
}
