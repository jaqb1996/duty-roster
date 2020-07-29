using ClassLibrary.DataAccess.CSV.Models;
using ClassLibrary.DataAccess.Helpers;
using ClassLibrary.Helpers;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ClassLibrary.DataAccess.CSV
{
    public partial class CSVDataAccess : IDataAccess
    {
        readonly FileHelper fileHelper;

        const string DataPath = "Data\\CSV";
        readonly string SchedulesFile = DataPath + "\\Schedules.csv";
        readonly string WorkingPlanFile = DataPath + "\\WorkingPlan.csv";
        readonly string EmployeesFile = DataPath + "\\Employees.csv";
        readonly string AllWorkingOptionsFile = DataPath + "\\AllWorkingOptions.csv";
        readonly string WorkingOptionsOfEmployeeFile = DataPath + "\\WorkingOptionsOfEmployee.csv";

        readonly string dateFormat = "yyyy-MM-dd";
        readonly string timeFormat = @"hh\:mm";
        readonly string defaultTime = "00:00";
        public CSVDataAccess()
        {
            fileHelper = new FileHelper(DataPath, SchedulesFile, WorkingPlanFile, EmployeesFile, AllWorkingOptionsFile, WorkingOptionsOfEmployeeFile, ',');
        }
        public int AddEmployee(string firstName, string lastName)
        {
            int id = GetNextID(EmployeesFile);
            fileHelper.WriteToFile(EmployeesFile, (writer) =>
            {
                writer.Write($"{id},{firstName},{lastName}");
            });
            return id;
        }

        public void AddEmployeeToSchedule(int scheduleID, int employeeID)
        {
            ISchedulePresentationData schedule = GetScheduleFromId(scheduleID);
            int numberOfDays = schedule.GetNumberOfDays();
            fileHelper.WriteToFile(WorkingPlanFile, (writer) =>
            {
                writer.Write($"{scheduleID},{employeeID}");
                for (int i = 0; i < numberOfDays; i++)
                {
                    writer.Write($",{""},{DateTime.Parse(defaultTime).ToString(timeFormat)},{TimeSpan.Parse(defaultTime).ToString(timeFormat)}");
                }
            });
        }

        public int AddWorkingOption(string symbol, DateTime startingHour, TimeSpan workingTime)
        {
            int id = GetNextID(AllWorkingOptionsFile);
            fileHelper.WriteToFile(AllWorkingOptionsFile, (writer) =>
             {
                 writer.Write($"{id},{symbol},{startingHour.ToString(timeFormat)},{workingTime.ToString(timeFormat)}");
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
            int id = GetNextID(SchedulesFile);
            fileHelper.WriteToFile(SchedulesFile, (writer) =>
            {
                writer.Write($"{id},{name},{startingDay.ToString(dateFormat)},{lastDay.ToString(dateFormat)}");
            });
            return id;
        }

        public ISchedule LoadSchedule(int id)
        {
            ISchedulePresentationData schedule = GetScheduleFromId(id);
            ISchedule output = new ScheduleModel(schedule.Id, schedule.Name, schedule.StartingDay, schedule.LastDay);

            List<WorkingPlan> plans = GetWorkingPlans();
            List<IEmployeePresentationData> employeeData = GetEmployees();
            List<IEmployee> employeesWithoutWorkingPlan = (from p in plans
                                                           where p.ScheduleID == output.Id
                                                           join e in employeeData
                                                           on p.EmployeeID equals e.Id
                                                           select output.CreateEmployee(e.Id, e.FirstName, e.LastName)).ToList();

            List<IEmployee> employees = new List<IEmployee>();
            List<WorkingOptionOfEmployee> workingOptionOfEmployees = GetWorkingOptionOfEmployees();
            List<WorkingOption> workingOptions = GetWorkingOptions();
            foreach (IEmployee employee in employeesWithoutWorkingPlan)
            {
                List<IWorkingOption> options = (from i in workingOptionOfEmployees
                                                where i.EmployeeID == employee.Id
                                                join o in workingOptions
                                                on i.EmployeeID equals o.Id
                                                select new WorkingOptionModel(o.Symbol, o.WorkingTime, o.StartingHour) as IWorkingOption).ToList();
                employees.Add(output.CreateEmployee(employee.Id, employee.FirstName, employee.LastName, options));
            }


            employees.ForEach(e => output.AddEmployee(e));

            foreach (IEmployee employee in employees)
            {
                List<IWorkingOption> workingPlan = (from p in plans
                                                    where p.EmployeeID == employee.Id && p.ScheduleID == output.Id
                                                    select p.WorkingOptions).Single();
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
                newPlans.Add(new WorkingPlan()
                {
                    ScheduleID = schedule.Id,
                    EmployeeID = emp.Id,
                    WorkingOptions = emp.WorkingPlan
                });
            }
            SaveWorkingPlans(newPlans);
        }
    }
}
