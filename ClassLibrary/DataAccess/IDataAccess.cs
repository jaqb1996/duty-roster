using ClassLibrary.Models;
using System;
using System.Collections.Generic;

namespace ClassLibrary.DataAccess
{
    public interface IDataAccess
    {
        /// <summary>
        /// Loads schedule from database
        /// </summary>
        /// <param name="id">Id of scbedule to load</param>
        /// <returns>Schedule loaded</returns>
        ScheduleModel LoadSchedule(int id);
        /// <summary>
        /// Reads information about shedules in database
        /// </summary>
        List<ISchedulePresentationData> ReadNamesOfAvailableSchedules();
        /// <summary>
        /// Reads information about employees in database
        /// </summary>
        List<IEmployeePresentationData> ReadNamesOfAvailableEmployees();
        /// <summary>
        /// Reads information about working options in database
        /// </summary>
        /// <returns></returns>
        List<IWorkingOption> ReadAvailableWorkingOptions();
        /// <summary>
        /// Creates new schedule in database
        /// </summary>
        /// <param name="name">Name of schedule</param>
        /// <param name="startingDay">First day of schedule</param>
        /// <param name="lastDay">Last day of schedule</param>
        /// <returns>Id of newly added schedule</returns>
        int CreateSchedule(string name, DateTime startingDay, DateTime lastDay);
        /// <summary>
        /// Adds new employee to database 
        /// </summary>
        /// <param name="firstName">First name of employee</param>
        /// <param name="lastName">Last name of employee</param>
        /// <returns>Id of newly added employee</returns>
        int AddEmployee(string firstName, string lastName);
        /// <summary>
        /// Adds employee to schedule
        /// </summary>
        /// <param name="scheduleID">Id of schedule</param>
        /// <param name="employeeID">Id of employee</param>
        void AddEmployeeToSchedule(int scheduleID, int employeeID);
        /// <summary>
        /// Adds new working option to database 
        /// </summary>
        /// <param name="Symbol">Symbol of working option</param>
        /// <param name="StartingHour">Time when working starts</param>
        /// <param name="WorkingTime">Duration of work</param>
        /// <returns>Id of newly added working option</returns>
        int AddWorkingOption(string Symbol, DateTime StartingHour, TimeSpan WorkingTime);
        /// <summary>
        /// Links working option to employee
        /// </summary>
        /// <param name="employeeID">Id of employee</param>
        /// <param name="workingOptionID">Id of working option</param>
        void AddWorkingOptionToEmployee(int employeeID, int workingOptionID);
        /// <summary>
        /// Changes working option for some day
        /// </summary>
        /// <param name="scheduleID">Id of shedule</param>
        /// <param name="employeeID">Id of employee</param>
        /// <param name="date">Day of work shift</param>
        /// <param name="workingOption">Working option specifying details of work shift</param>
        void AddWorkingOptionForDay(int scheduleID, int employeeID, DateTime date, IWorkingOption workingOption);
        //List<IWorkingOption> GetWorkingOptionsForEmployee(int employeeID);
        /// <summary>
        /// Saves schedule in database
        /// </summary>
        /// <param name="schedule">Schedule to save</param>
        void SaveSchedule(ISchedule schedule);
    }
}