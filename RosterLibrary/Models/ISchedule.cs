using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosterLibrary.Models
{
    public interface ISchedule : ISchedulePresentationData
    {
        List<IEmployee> Employees { get; }
        //List<DateTime> WorkingDays { get; }
        IEmployee CreateEmployee(int id, string firstName, string lastName);
        IEmployee CreateEmployee(int id, string firstName, string lastName, List<IWorkingOption> workingOptions);
        void AddEmployee(IEmployee employee);
        void ChangeWorkDay(int employeeID, DateTime date, string symbol, DateTime startingHour, TimeSpan workingTime);
        void IterateOverAllDays(Action<DateTime> actionForDay);
        IWorkingOption PlanForDay(int employeeID, DateTime date); 
    }
}
