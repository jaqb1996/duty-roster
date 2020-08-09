using ClassLibrary.DataAccess.CSV.Models;
using ClassLibrary.DataAccess.Helpers;
using ClassLibrary.Helpers;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClassLibrary.DataAccess.CSV
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
            List<IEmployee> employeesWithoutWorkingPlan = (from p in plans
                                                           where p.ScheduleID == output.Id
                                                           join e in employeeData
                                                           on p.EmployeeID equals e.Id
                                                           select output.CreateEmployee(e.Id, e.FirstName, e.LastName)).ToList();

            // Fill in the list of employees with available options
            List<IEmployee> employees = new List<IEmployee>();
            List<WorkingOptionOfEmployee> workingOptionOfEmployees = GetWorkingOptionOfEmployees();
            List<WorkingOption> workingOptions = GetWorkingOptions();
            foreach (IEmployee employee in employeesWithoutWorkingPlan)
            {
                List<IWorkingOption> options = (from i in workingOptionOfEmployees
                                                where i.EmployeeID == employee.Id
                                                join o in workingOptions
                                                on i.WorkingOptionID equals o.Id
                                                select new WorkingOptionModel(o.Symbol, o.WorkingTime, o.StartingHour) as IWorkingOption).ToList();
                employees.Add(output.CreateEmployee(employee.Id, employee.FirstName, employee.LastName, options));
            }

            // Add each employee to output schedule
            employees.ForEach(e => output.AddEmployee(e));

            // Get working plan for each employee
            foreach (IEmployee employee in employees)
            {
                List<IWorkingOption> workingPlan = (from p in plans
                                                    where p.EmployeeID == employee.Id && p.ScheduleID == output.Id
                                                    select p.WorkingOptions).Single();

                // Fill in output schedule with plans for each day
                int i = 0;
                output.IterateOverAllDays((day) =>
                {
                    IWorkingOption planForDay = workingPlan[i++];
                    output.ChangeWorkDay(employee.Id, day, planForDay.Symbol, planForDay.StartingHour, planForDay.WorkingTime);
                });
            }
            return output;
        }

        public List<IWorkingOption> ReadAvailableWorkingOptions()
        {
            List<IWorkingOption> output = new List<IWorkingOption>();
            foreach (var option in GetWorkingOptions())
            {
                output.Add(option);
            }
            return output;
        }

        public List<IEmployeePresentationData> ReadNamesOfAvailableEmployees()
        {
            List<IEmployeePresentationData> output = new List<IEmployeePresentationData>();

            fileHelper.ReadFile(EmployeesFile, (lineData) =>
            {
                output.Add(new Employee { Id = int.Parse(lineData[0]), FirstName = lineData[1], LastName = lineData[2] });
            });

            return output;
        }

        public List<ISchedulePresentationData> ReadNamesOfAvailableSchedules()
        {
            List<ISchedulePresentationData> output = new List<ISchedulePresentationData>();

            fileHelper.ReadFile(SchedulesFile, (lineData) =>
            {
                output.Add(new Schedule { Id = int.Parse(lineData[0]), Name = lineData[1], 
                    StartingDay = DateTime.Parse(lineData[2]), LastDay = DateTime.Parse(lineData[3]) });
            });

            return output;
        }

        public void SaveSchedule(ISchedule schedule)
        {
            List<WorkingPlan> oldPlans = GetWorkingPlans();
            List<WorkingPlan> newPlans = new List<WorkingPlan>();
            foreach (WorkingPlan plan in oldPlans)
            {
                if (plan.ScheduleID == schedule.Id) continue;
                newPlans.Add(plan);
            }
            foreach (IEmployee emp in schedule.Employees)
            {
                // Validate symbol for each plan
                emp.WorkingPlan.ForEach(o => CheckInputStrings(SymbolPattern, o.Symbol));
                newPlans.Add(new WorkingPlan()
                {
                    ScheduleID = schedule.Id,
                    EmployeeID = emp.Id,
                    WorkingOptions = emp.WorkingPlan
                });
            }
            SaveWorkingPlans(newPlans);
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
            List<WorkingPlan> oldPlan = GetWorkingPlans();
            List<WorkingPlan> newPlan = new List<WorkingPlan>();
            oldPlan.Where(x => !(x.EmployeeID == employeeID && x.ScheduleID == scheduleID)).ToList().ForEach(x => newPlan.Add(x));
            SaveWorkingPlans(newPlan);
        }
        public void DeleteEmployee(int employeeID)
        {
            throw new NotImplementedException();
        }
        
    }
}
