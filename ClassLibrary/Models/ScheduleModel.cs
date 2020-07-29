﻿using ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public partial class ScheduleModel : ISchedule
    {
        public int Id { get; }
        public string Name { get; }
        public DateTime StartingDay { get; }
        public DateTime LastDay { get; }
        public List<IEmployee> Employees { get; }
        //public List<DateTime> WorkingDays { get; }


        public ScheduleModel(int id, string name, DateTime startingDay, DateTime lastDay)
        {
            Id = id;
            Name = name;
            if (!(startingDay < lastDay))
                throw new Exception("Starting day must be earlier than last day");
            StartingDay = startingDay;
            LastDay = lastDay;
            Employees = new List<IEmployee>();
            //WorkingDays = new List<DateTime>();
            //this.IterateOverAllDays(day => { WorkingDays.Add(day); });

        }
        // Only ScheduleModel can create new instances of EmployeeModel, because number of days is required.
        public IEmployee CreateEmployee(int id, string firstName, string lastName)
        {
            return new EmployeeModel(id, firstName, lastName, this.GetNumberOfDays());
        }
        public IEmployee CreateEmployee(int id, string firstName, string lastName, List<IWorkingOption> workingOptions)
        {
            return new EmployeeModel(id, firstName, lastName, this.GetNumberOfDays(), workingOptions);
        }
        public void AddEmployee(IEmployee employee)
        {
            if (Employees.Contains(employee))
            {
                throw new Exception("Employee already exists in this schedule");
            }
            Employees.Add(employee);
        }
        public void ChangeWorkDay( int employeeID, DateTime date, string symbol, DateTime startingHour, TimeSpan workingTime)
        {
            int employeeIndex = Employees.FindIndex(0, e => e.Id == employeeID);
            if (employeeIndex == -1)
                throw new Exception("Such employee does not exist in the schedule");
            int index = this.GetIndexOfDate(date);
            Employees[employeeIndex].ChangeWorkDay(index, symbol, startingHour, workingTime);
        }
    }
}