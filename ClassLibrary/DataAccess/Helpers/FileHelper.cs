using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary.DataAccess.Helpers
{
    class FileHelper
    {
        public FileHelper(string dataPath, string SchedulesFile, string WorkingPlanFile, string EmployeesFile, 
                          string AllWorkingOptionsFile, string WorkingOptionsOfEmployeeFile, char separator)
        {

            string DataPath = dataPath;
            string SchedulesFilePath = SchedulesFile;
            string WorkingPlanFilePath = WorkingPlanFile;
            string EmployeesFilePath = EmployeesFile;
            string AllWorkingOptionsFilePath = AllWorkingOptionsFile;
            string WorkingOptionsOfEmployeeFilePath = WorkingOptionsOfEmployeeFile;

            Directory.CreateDirectory(DataPath);

            CreateFile(SchedulesFilePath);
            CreateFile(WorkingPlanFilePath);
            CreateFile(EmployeesFilePath);
            CreateFile(AllWorkingOptionsFilePath);
            CreateFile(WorkingOptionsOfEmployeeFilePath);

            this.separator = separator;

        }
        readonly char separator;
        //readonly string DataPath;
        //readonly string SchedulesFilePath;
        //readonly string WorkingPlanFilePath;
        //readonly string EmployeesFilePath;
        //readonly string AllWorkingOptionsFilePath;
        //readonly string WorkingOptionsOfEmployeeFilePath;


        private void CreateFile(string pathToFile)
        {
            if (!File.Exists(pathToFile))
                File.WriteAllText(pathToFile, "");
        }
        public void WriteToFile(string filePath, Action<StreamWriter> action)
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                action.Invoke(writer);
                writer.WriteLine();
            }
        }
        public void DeleteContentOfFile(string filePath)
        {
            File.WriteAllText(filePath, "");
        }
        public void ReadFile(string filePath, Action<string[]> action)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineData = line.Split(separator);
                    action.Invoke(lineData);
                }
                
            }
        }
    }
}
