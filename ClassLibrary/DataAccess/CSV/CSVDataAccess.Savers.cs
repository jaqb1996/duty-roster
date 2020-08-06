using ClassLibrary.DataAccess.CSV.Models;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DataAccess.CSV
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
    }
}
