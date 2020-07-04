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
            //XmlDataAccess dataAccess = new XmlDataAccess();
            //var files = dataAccess.ReadNamesOfAvailableSchedules();
            //foreach (var f in files)
            //{
            //    Console.WriteLine(f);
            //}

            //ScheduleModel schedule = new ScheduleModel("Test", DateTime.Parse("1.1.2020"), DateTime.Parse("3.1.2020"));
            //ScheduleModel schedule;
            //while (true)
            //{
            //    bool newSchedule = ShowListOfSchedules(out int choice, out List<string> scheduleNames);

            //    if (newSchedule)
            //    {
            //        //schedule.AddEmployee("Zenek", "Blues");
            //        //Console.WriteLine(schedule.FullSchedule());
            //        //dataAccess.SaveSchedule(GetNewSchedule());
            //    }
            //    else
            //    {
            //        schedule = dataAccess.LoadSchedule(scheduleNames[choice]);
            //        break;
            //        //Console.WriteLine(schedule.FullSchedule());
            //    }
            //}
            //Console.WriteLine(schedule.FullSchedule());

            //dataAccess.CreateSchedule("abc", new DateTime(2020, 01, 01), new DateTime(2020, 01, 10));
            //dataAccess.CreateSchedule("xyz", new DateTime(2019, 02, 01), new DateTime(2019, 01, 15));
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

            ScheduleModel schedule = dataAccess.LoadSchedule(scheduleID);

            Console.WriteLine(schedule.Name);

            schedule.Employees.ForEach(e => Console.WriteLine(e.LastName));

            schedule.ChangeWorkDay(id_Wilk, new DateTime(2020, 02, 04), "R", DateTime.Parse("6:50"), TimeSpan.Parse("12:00"));

            dataAccess.SaveSchedule(schedule);

            //ScheduleModel schedule = dataAccess.LoadSchedule();
            //schedule.WorkingDays.ForEach(x => Console.WriteLine(x));

            Console.WriteLine("Nacisnij dowolny przycisk aby zamknac program...");
            Console.ReadKey();
        }

        //private static ScheduleModel GetNewSchedule()
        //{
        //    Console.WriteLine("Podaj nazwe nowego grafiku: ");
        //    string newName = Console.ReadLine();
        //    Console.WriteLine("Podaj date poczatkowa (dd.mm.rrrr): ");
        //    DateTime startDate = DateTime.Parse(Console.ReadLine());
        //    Console.WriteLine("Podaj date koncowa (dd.mm.rrrr): ");
        //    DateTime lastDate = DateTime.Parse(Console.ReadLine());
        //    return new ScheduleModel(newName, startDate, lastDate);
        //}

        //private static bool ShowListOfSchedules(out int choice, out List<string> scheduleNames)
        //{
        //    Console.WriteLine("Lista dostępnych grafików:");
        //    scheduleNames = dataAccess.ReadNamesOfAvailableSchedules();
        //    while (true)
        //    {
        //        for (int i = 0; i < scheduleNames.Count; i++)
        //        {
        //            Console.WriteLine($"[{i}] {scheduleNames[i]}");
        //        }
        //        Console.WriteLine($"[{scheduleNames.Count}] Nowy grafik");
        //        bool success = int.TryParse(Console.ReadLine(), out choice);
                
        //        if (success && 0 <= choice && choice < scheduleNames.Count)
        //        {
        //            return false;
        //        }
        //        if (choice == scheduleNames.Count)
        //            return true;
        //        Console.WriteLine("Niepoprawny wybor. Sprobuj ponownie");
        //    }
        //}
    }
}
