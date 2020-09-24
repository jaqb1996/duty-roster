using RosterLibrary.DataAccess.CSV.Models;
using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosterLibrary.DataAccess
{
    class CommonFunctionality
    {
        // Action<> does not support params. Own delegate needed.
        public delegate void InputStringsChecker(string pattern, params string[] inputStrings);
        public void SaveSchedule(ISchedule schedule, Func<List<WorkingPlan>> GetWorkingPlans, Action<List<WorkingPlan>> SaveWorkingPlans, InputStringsChecker CheckInputStrings, string SymbolPattern)
        {
            List<WorkingPlan> oldPlans = GetWorkingPlans.Invoke();
            List<WorkingPlan> newPlans = new List<WorkingPlan>();
            foreach (WorkingPlan plan in oldPlans)
            {
                if (plan.ScheduleID == schedule.Id) continue;
                newPlans.Add(plan);
            }
            foreach (IEmployee emp in schedule.Employees)
            {
                // Validate symbol for each plan
                emp.WorkingPlan.ForEach(o => CheckInputStrings.Invoke(SymbolPattern, o.Symbol));
                newPlans.Add(new WorkingPlan()
                {
                    ScheduleID = schedule.Id,
                    EmployeeID = emp.Id,
                    WorkingOptions = emp.WorkingPlan
                });
            }
            SaveWorkingPlans.Invoke(newPlans);
        }
    }
}
