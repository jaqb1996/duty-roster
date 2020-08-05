using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Helpers
{
    static class ScheduleHelper
    {
        public static int GetNumberOfDays(this ISchedulePresentationData schedule)
        {   
            return (schedule.LastDay - schedule.StartingDay).Days + 1;
        }
        public static int GetIndexOfDate(this ISchedulePresentationData schedule, DateTime date)
        {
            int i = 0;
            for (DateTime day = schedule.StartingDay; day <= schedule.LastDay; day = day.AddDays(1))
            {
                if (day.Day == date.Day && day.Month == date.Month && day.Year == date.Year)
                    return i;
                i++;
            }
            throw new ArgumentException("Such date does not exist in the schedule");
        }
    }
}
