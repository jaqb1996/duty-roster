using RosterLibrary.DataAccess.Models;
using RosterLibrary.DataAccess.Helpers;
using RosterLibrary.Helpers;
using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RosterLibrary.DataAccess.CSV
{
    public partial class CSVDataAccess : IDataAccess
    {
        readonly FileHelper fileHelper;

        // Do not let user enter dangerous characters for csv files
        public const string NamePattern = @"^\p{L}[^,\n=\-+@]*$"; // Names must begin with letter  
        public const string SymbolPattern = @"^[^,\n=\-+@]*$"; // Symbol can be empty

        const string DataPath = "Data\\CSV";
        readonly string SchedulesFile = DataPath + "\\Schedules.csv";
        readonly string WorkingPlanFile = DataPath + "\\WorkingPlan.csv";
        readonly string EmployeesFile = DataPath + "\\Employees.csv";
        readonly string AllWorkingOptionsFile = DataPath + "\\AllWorkingOptions.csv";
        readonly string WorkingOptionsOfEmployeeFile = DataPath + "\\WorkingOptionsOfEmployee.csv";

        readonly string dateFormat = "yyyy-MM-dd";
        readonly string defaultTime = "00:00";

        CommonFunctionality common = new CommonFunctionality();
        public CSVDataAccess()
        {
            fileHelper = new FileHelper(DataPath, SchedulesFile, WorkingPlanFile, EmployeesFile, AllWorkingOptionsFile, WorkingOptionsOfEmployeeFile, ',');
        }
        public int AddEmployee(string firstName, string lastName)
        {
            CheckInputStrings(NamePattern, firstName, lastName);
            int id = GetNextID(EmployeesFile);
            fileHelper.WriteToFile(EmployeesFile, (writer) =>
            {
                writer.Write($"{id},{firstName},{lastName}");
            });
            return id;
        }

        public void AddEmployeeToSchedule(int scheduleID, int employeeID)
        {
            //ISchedulePresentationData schedule = GetScheduleFromId(scheduleID);
            ISchedule schedule = LoadSchedule(scheduleID);

            // Check if schedule already contains such employee
            if (schedule.Employees.Select(e => e.Id).Contains(employeeID))
                throw new InvalidOperationException("Employee already exists in the schedule");

            // Save default plan for employee
            int numberOfDays = schedule.GetNumberOfDays();
            fileHelper.WriteToFile(WorkingPlanFile, (writer) =>
            {
                writer.Write($"{scheduleID},{employeeID}");
                for (int i = 0; i < numberOfDays; i++)
                {
                    writer.Write($",{WorkingOptionModel.DefaultSymbol},{DateTime.Parse(defaultTime).ToString(WorkingOptionModel.StartingHourFormat)},{TimeSpan.Parse(defaultTime).ToString(WorkingOptionModel.WorkingTimeFormat)}");
                }
            });
        }

        public int AddWorkingOption(string symbol, DateTime startingHour, TimeSpan workingTime)
        {
            CheckInputStrings(SymbolPattern, symbol);
            int id = GetNextID(AllWorkingOptionsFile);
            fileHelper.WriteToFile(AllWorkingOptionsFile, (writer) =>
             {
                 writer.Write($"{id},{symbol},{startingHour.ToString(WorkingOptionModel.StartingHourFormat)},{workingTime.ToString(WorkingOptionModel.WorkingTimeFormat)}");
             });
            return id;
        }

        public void AddWorkingOptionToEmployee(int employeeID, int workingOptionID)
        {
            fileHelper.WriteToFile(WorkingOptionsOfEmployeeFile, (writer) =>
            {
                writer.Write($"{employeeID},{workingOptionID}");
            });
        }

        public void AddWorkingOptionForDay(int scheduleID, int employeeID, DateTime date, IWorkingOption workingOption)
        {
            CheckInputStrings(SymbolPattern, workingOption.Symbol);
            ISchedulePresentationData schedule = GetScheduleFromId(scheduleID);
            int indexOfDate = schedule.GetIndexOfDate(date);

            List<WorkingPlan> workingPlans = GetWorkingPlans();

            foreach (WorkingPlan plan in workingPlans)
            {
                if (plan.ScheduleID == scheduleID && plan.EmployeeID == employeeID)
                {
                    for (int i = 0; i < plan.WorkingOptions.Count; i++)
                    {
                        if (i == indexOfDate)
                        {
                            plan.WorkingOptions[i] = workingOption;
                            break;
                        }
                    }
                }
            }

            SaveWorkingPlans(workingPlans);

        }

        public int CreateSchedule(string name, DateTime startingDay, DateTime lastDay)
        {
            CheckInputStrings(NamePattern, name);
            if (!(startingDay < lastDay))
                throw new ArgumentException("Starting day must be earlier then last day");
            int id = GetNextID(SchedulesFile);
            fileHelper.WriteToFile(SchedulesFile, (writer) =>
            {
                writer.Write(GetScheduleDataSeparated(id, name, startingDay, lastDay));
            });
            return id;
        }

        public ISchedule LoadSchedule(int id)
        {
            // Get basic information about schedule
            ISchedulePresentationData schedule = GetScheduleFromId(id);
            ISchedule output = new ScheduleModel(schedule.Id, schedule.Name, schedule.StartingDay, schedule.LastDay);

            // Get list of employees (without available working options for now)
            List<WorkingPlan> plans = GetWorkingPlans();
            List<IEmployeePresentationData> employeeData = GetEmployees();
            List<IEmployee> employeesWithoutWorkingPlan = common.GetEmployeesWithoutWorkingPlan(plans, employeeData, output);

            // Fill in the list of employees with available options
            List<IEmployee> employees = new List<IEmployee>();
            List<WorkingOptionOfEmployee> workingOptionOfEmployees = GetWorkingOptionOfEmployees();
            List<IWorkingOption> workingOptions = GetWorkingOptions();
            common.FillEmployeesWithWorkingOptions(employees, employeesWithoutWorkingPlan, workingOptionOfEmployees, workingOptions, output);

            // Add each employee to output schedule
            employees.ForEach(e => output.AddEmployee(e));

            // Get working plan for each employee
            common.GetWorkingPlanForEmployees(employees, plans, output);
            return output;
        }

        public List<IWorkingOption> ReadAvailableWorkingOptions()
        {
            return GetWorkingOptions();
        }

        public List<IEmployeePresentationData> ReadNamesOfAvailableEmployees()
        {
            return GetEmployees();
        }

        public List<ISchedulePresentationData> ReadNamesOfAvailableSchedules()
        {
            return GetSchedules();
        }

        public void SaveSchedule(ISchedule schedule)
        {
            common.SaveSchedule(schedule, GetWorkingPlans, SaveWorkingPlans, CheckInputStrings, SymbolPattern);
        }

        public void DeleteSchedule(int scheduleID)
        {
            // Delete from WorkingPlanFile
            List<WorkingPlan> oldPlan = GetWorkingPlans();
            List<WorkingPlan> newPlan = new List<WorkingPlan>();
            oldPlan.Where(x => x.ScheduleID != scheduleID).ToList().ForEach(x => newPlan.Add(x));
            SaveWorkingPlans(newPlan);

            // Delete from SchedulesFile
            List<ISchedulePresentationData> oldSchedules = GetSchedules();
            List<Schedule> newSchedules = new List<Schedule>();
            oldSchedules.Where(s => s.Id != scheduleID).ToList().ForEach(s => newSchedules.Add(s as Schedule));
            SaveSchedulesToFile(newSchedules);

        }
        public void DeleteEmployeeFromSchedule(int employeeID, int scheduleID)
        {
            // Delete from WorkingPlanFile
            List<WorkingPlan> oldPlan = GetWorkingPlans();
            List<WorkingPlan> newPlan = new List<WorkingPlan>();
            oldPlan.Where(x => !(x.EmployeeID == employeeID && x.ScheduleID == scheduleID)).ToList().ForEach(x => newPlan.Add(x));
            SaveWorkingPlans(newPlan);
        }
        public void DeleteEmployee(int employeeID)
        {
            // Delete from WorkingPlanFile
            List<WorkingPlan> oldPlans = GetWorkingPlans();
            List<WorkingPlan> newPlans = new List<WorkingPlan>();
            oldPlans.Where(x => x.EmployeeID != employeeID).ToList().ForEach(x => newPlans.Add(x));
            SaveWorkingPlans(newPlans);

            // Delete from WorkingOptionOfEmployeeFile
            List<WorkingOptionOfEmployee> workingOptions = GetWorkingOptionOfEmployees();
            List<WorkingOptionOfEmployee> newWorkingOptions = new List<WorkingOptionOfEmployee>();
            workingOptions.Where(x => x.EmployeeID != employeeID).ToList().ForEach(x => newWorkingOptions.Add(x));
            SaveWorkingOptionsOfEmployee(newWorkingOptions);

            // Delete from EmployeesFile
            List<IEmployeePresentationData> employees = GetEmployees();
            List<Employee> newEmployees = new List<Employee>();
            employees.Where(x => x.Id != employeeID).ToList().ForEach(x => newEmployees.Add(x as Employee));
            SaveEmployees(newEmployees);
        }

        public void CheckScheduleSymbol(string symbol)
        {
            CheckInputStrings(SymbolPattern, symbol);
        }
    }
}
