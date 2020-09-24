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
        //public ISchedule LoadSchedule(int id, Func<int, ISchedulePresentationData> GetScheduleFromID, Func<List<WorkingPlan>> GetWorkingPlans,
        //    Func<List<IEmployeePresentationData>> GetEmployees, Func<List<WorkingOptionOfEmployee>> GetWorkingOptionsOfEmployees, Func<>)
        public List<IEmployee> GetEmployeesWithoutWorkingPlan(List<WorkingPlan> plans, List<IEmployeePresentationData> employeeData, ISchedule schedule)
        {
            List<IEmployee> output = (from p in plans
                                    where p.ScheduleID == schedule.Id
                                    join e in employeeData
                                    on p.EmployeeID equals e.Id
                                    select schedule.CreateEmployee(e.Id, e.FirstName, e.LastName)).ToList();
            return output;
        }
        public void FillEmployeesWithWorkingOptions(List<IEmployee> employees, List<IEmployee> employeesWithoutWorkingPlan, 
                                                    List<WorkingOptionOfEmployee> workingOptionOfEmployees,
                                                    List<IWorkingOption> workingOptions, ISchedule schedule)
        {
            foreach (IEmployee employee in employeesWithoutWorkingPlan)
            {
                List<IWorkingOption> options = (from i in workingOptionOfEmployees
                                                where i.EmployeeID == employee.Id
                                                join o in workingOptions
                                                on i.WorkingOptionID equals o.Id
                                                select new WorkingOptionModel(o.Symbol, o.WorkingTime, o.StartingHour) as IWorkingOption).ToList();
                employees.Add(schedule.CreateEmployee(employee.Id, employee.FirstName, employee.LastName, options));
            }
        }
        public void GetWorkingPlanForEmployees(List<IEmployee> employees, List<WorkingPlan> plans, ISchedule schedule)
        {
            foreach (IEmployee employee in employees)
            {
                List<IWorkingOption> workingPlan = (from p in plans
                                                    where p.EmployeeID == employee.Id && p.ScheduleID == schedule.Id
                                                    select p.WorkingOptions).Single();

                // Fill in output schedule with plans for each day
                int i = 0;
                schedule.IterateOverAllDays((day) =>
                {
                    IWorkingOption planForDay = workingPlan[i++];
                    schedule.ChangeWorkDay(employee.Id, day, planForDay.Symbol, planForDay.StartingHour, planForDay.WorkingTime);
                });
            }
        }
    }
}
