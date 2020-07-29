using ClassLibrary.DataAccess;
using ClassLibrary.DataAccess.CSV;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestingApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Directory.Exists("Data"))
            {
                Directory.Delete("Data", true);
            } 

            IDataAccess dataAccess = new CSVDataAccess();

            int scheduleID = dataAccess.CreateSchedule("February", new DateTime(2020, 02, 01), new DateTime(2020, 02, 15));
            int id_Wilk = dataAccess.AddEmployee("Jacek", "Wilk");
            int id_Iksinski = dataAccess.AddEmployee("Zenon", "Iksiński");
            dataAccess.AddEmployee("Marek", "Mikulski");
            List<ISchedulePresentationData> scheduleData = dataAccess.ReadNamesOfAvailableSchedules();
            Console.WriteLine("Available schedules:");
            scheduleData.ForEach(x => Console.WriteLine($"{x.Id}, {x.Name}"));

            List<IEmployeePresentationData> employeeData = dataAccess.ReadNamesOfAvailableEmployees();
            Console.WriteLine("Available employees");
            employeeData.ForEach(x => Console.WriteLine($"{x.Id}, {x.FirstName} {x.LastName}"));

            int id_R = dataAccess.AddWorkingOption("R", DateTime.Parse("7:00"), TimeSpan.Parse("7:35"));
            dataAccess.AddWorkingOption("D", DateTime.Parse("7:00"), TimeSpan.Parse("12:00"));
            List<IWorkingOption> workingOptions = dataAccess.ReadAvailableWorkingOptions();
            workingOptions.ForEach(x => Console.WriteLine($"{x.Id},{x.Symbol},{x.StartingHour},{x.WorkingTime}"));

            dataAccess.AddEmployeeToSchedule(scheduleID, id_Wilk);
            dataAccess.AddEmployeeToSchedule(scheduleID, id_Iksinski);

            dataAccess.AddWorkingOptionToEmployee(id_Wilk, id_R);
            dataAccess.AddWorkingOptionToEmployee(id_Iksinski, id_R);

            dataAccess.AddWorkingOptionForDay(scheduleID, id_Wilk, new DateTime(2020, 02, 02), workingOptions.First());
            dataAccess.AddWorkingOptionForDay(scheduleID, id_Iksinski, new DateTime(2020, 02, 03), workingOptions.First());

            ISchedule schedule = dataAccess.LoadSchedule(scheduleID);

            Console.WriteLine(schedule.Name);

            schedule.Employees.ForEach(e => Console.WriteLine(e.LastName));

            schedule.ChangeWorkDay(id_Wilk, new DateTime(2020, 02, 04), "R", DateTime.Parse("6:50"), TimeSpan.Parse("12:00"));

            dataAccess.SaveSchedule(schedule);

            //ScheduleModel schedule = dataAccess.LoadSchedule();
            //schedule.WorkingDays.ForEach(x => Console.WriteLine(x));

            Console.WriteLine("Nacisnij dowolny przycisk aby zamknac program...");
            Console.ReadKey();
        }
    }
}
