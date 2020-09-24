using RosterLibrary.DataAccess.CSV.Models;
using RosterLibrary.Helpers;
using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RosterLibrary.DataAccess.CSV
{
    public partial class CSVDataAccess
    {
        private List<IEmployeePresentationData> GetEmployees()
        {
            List<IEmployeePresentationData> output = new List<IEmployeePresentationData>();
            fileHelper.ReadFile(EmployeesFile, (lineData) =>
            {
                int id = int.Parse(lineData[0]);
                string firstName = lineData[1];
                string lastName = lineData[2];
                output.Add(new Employee { Id = id, FirstName = firstName, LastName = lastName });
            });
            return output;
        }
        private List<ISchedulePresentationData> GetSchedules()
        {
            List<ISchedulePresentationData> output = new List<ISchedulePresentationData>();
            fileHelper.ReadFile(SchedulesFile, (lineData) =>
            {
                int id = int.Parse(lineData[0]);
                string name = lineData[1];
                DateTime startingDate = DateTime.Parse(lineData[2]);
                DateTime lastDate = DateTime.Parse(lineData[3]);
                output.Add(new Schedule { Id = id, Name = name, StartingDay = startingDate, LastDay = lastDate });
            });
            return output;
        }
        private List<IWorkingOption> GetWorkingOptions()
        {
            List<IWorkingOption> output = new List<IWorkingOption>();
            fileHelper.ReadFile(AllWorkingOptionsFile, (lineData) =>
            {
                int id = int.Parse(lineData[0]);
                string symbol = lineData[1];
                DateTime startingHour = DateTime.ParseExact(lineData[2], WorkingOptionModel.StartingHourFormat, CultureInfo.InvariantCulture);
                TimeSpan workingHours = TimeSpan.ParseExact(lineData[3], WorkingOptionModel.WorkingTimeFormat, CultureInfo.InvariantCulture);
                output.Add(new WorkingOption { Id = id, Symbol = symbol, StartingHour = startingHour, WorkingTime = workingHours });
            });
            return output;
        }
        private List<WorkingOptionOfEmployee> GetWorkingOptionOfEmployees()
        {
            List<WorkingOptionOfEmployee> output = new List<WorkingOptionOfEmployee>();
            fileHelper.ReadFile(WorkingOptionsOfEmployeeFile, (lineData) =>
            {
                int employeeID = int.Parse(lineData[0]);
                int workingOptionID = int.Parse(lineData[1]);
                output.Add(new WorkingOptionOfEmployee { EmployeeID = employeeID, WorkingOptionID = workingOptionID });
            });
            return output;
        }
        private List<WorkingPlan> GetWorkingPlans()
        {
            List<WorkingPlan> output = new List<WorkingPlan>();

            fileHelper.ReadFile(WorkingPlanFile, (lineData) =>
            {
                int scheduleID = int.Parse(lineData[0]);
                int employeeID = int.Parse(lineData[1]);
                
                int numberOfDaysInSchedule = (from schedule in GetSchedules() 
                                             where schedule.Id == scheduleID 
                                             select schedule.GetNumberOfDays()).Single();
                List<IWorkingOption> workingOptions = new List<IWorkingOption>();
                for (int i = 2; i < 3 * numberOfDaysInSchedule + 2; i += 3)
                {
                    string symbol = lineData[i];
                    DateTime startingHour = DateTime.Parse(lineData[i + 1]);
                    TimeSpan workingTime = TimeSpan.Parse(lineData[i + 2]);  
                    workingOptions.Add(new WorkingOption { Symbol = symbol, StartingHour = startingHour, WorkingTime = workingTime});
                }
                output.Add(new WorkingPlan { ScheduleID = scheduleID, EmployeeID = employeeID, WorkingOptions = workingOptions});
            });
            return output;
        }
        private int GetNextID(string pathOfFile, int idPosition = 0)
        {
            List<int> listOfIds = new List<int> { 0 };

            fileHelper.ReadFile(pathOfFile, (lineData) =>
            {
                listOfIds.Add(int.Parse(lineData[idPosition]));
            });
            
            return listOfIds.Max() + 1;
        }
        private ISchedulePresentationData GetScheduleFromId(int id)
        {
            return (from s in GetSchedules()
                    where id == s.Id
                    select s).Single();
        }
        public static void CheckInputStrings(string pattern, params string[] inputStrings)
        {
            foreach (string text in inputStrings)
            {
                if (!Regex.IsMatch(text, pattern))
                    throw new FormatException("Bad format of data");
            }
                
        } 
        private string GetScheduleDataSeparated(int id, string name, DateTime startingDay, DateTime lastDay)
        {
            return $"{id}{fileHelper.separator}{name}{fileHelper.separator}{startingDay.ToString(dateFormat)}{fileHelper.separator}{lastDay.ToString(dateFormat)}";
        }
    }
}
