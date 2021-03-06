﻿using RosterLibrary.DataAccess.Models;
using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace RosterLibrary.DataAccess.CSV
{
    public partial class CSVDataAccess
    {
        private void SaveWorkingPlans(List<WorkingPlan> workingPlans)
        {
            fileHelper.DeleteContentOfFile(WorkingPlanFile);
            foreach (WorkingPlan plan in workingPlans)
            {
                fileHelper.WriteToFile(WorkingPlanFile, (writer) =>
                {
                    writer.Write($"{plan.ScheduleID},{plan.EmployeeID}");
                    foreach (IWorkingOption option in plan.WorkingOptions)
                    {
                        writer.Write($",{option.Symbol},{option.StartingHour.ToString(WorkingOptionModel.StartingHourFormat)},{option.WorkingTime.ToString(WorkingOptionModel.WorkingTimeFormat)}");
                    }
                });
            }
        }
        private void SaveEmployees(List<Employee> employees)
        {
            fileHelper.DeleteContentOfFile(EmployeesFile);
            foreach (Employee emp in employees)
            {
                fileHelper.WriteToFile(EmployeesFile, (writer) =>
                {
                    writer.Write($"{emp.Id},{emp.FirstName},{emp.LastName}");
                });
            }
        }
        private void SaveSchedulesToFile(List<Schedule> schedules)
        {
            fileHelper.DeleteContentOfFile(SchedulesFile);
            foreach (Schedule schedule in schedules)
            {
                fileHelper.WriteToFile(SchedulesFile, (writer) =>
                {
                    writer.Write(GetScheduleDataSeparated(schedule.Id, schedule.Name, schedule.StartingDay, schedule.LastDay));
                });
            }
        }
        private void SaveWorkingOptionsOfEmployee(List<WorkingOptionOfEmployee> workingOptions)
        {
            fileHelper.DeleteContentOfFile(WorkingOptionsOfEmployeeFile);
            foreach (WorkingOptionOfEmployee pair in workingOptions)
            {
                AddWorkingOptionToEmployee(pair.EmployeeID, pair.WorkingOptionID);
            }
        }
    }
}
