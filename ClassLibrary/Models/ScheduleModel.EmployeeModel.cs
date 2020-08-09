using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public partial class ScheduleModel
    {
        class EmployeeModel : IEmployee
        {
            public int Id { get; }
            public string FirstName { get; }
            public string LastName { get; }
            public string FullName
            {
                get
                {
                    return $"{FirstName} {LastName}";
                }
            }

            public List<IWorkingOption> WorkingPlan { get; }

            public List<IWorkingOption> AvailableOptions { get; }

            public EmployeeModel(int id, string firstName, string lastName, int numberOFWorkingDays)
            {
                Id = id;
                FirstName = firstName;
                LastName = lastName;
                WorkingPlan = new List<IWorkingOption>();
                for (int i = 0; i < numberOFWorkingDays; i++)
                {
                    WorkingPlan.Add(new WorkingOptionModel());
                }
            }
            public EmployeeModel(int id, string firstName, string lastName, int numberOFWorkingDays, List<IWorkingOption> availableOptions) 
                : this(id, firstName, lastName, numberOFWorkingDays)
            {
                AvailableOptions = availableOptions;
            }
            public void ChangeWorkDay(int indexOfDate, string symbol, DateTime startingHour, TimeSpan workingTime)
            {
                WorkingPlan[indexOfDate].Symbol = symbol;
                WorkingPlan[indexOfDate].StartingHour = startingHour;
                WorkingPlan[indexOfDate].WorkingTime = workingTime;
            }

            public bool Equals(IEmployeePresentationData other)
            {
                return other != null &&
                       Id == other.Id &&
                       FirstName == other.FirstName &&
                       LastName == other.LastName;
            }

            public override int GetHashCode()
            {
                int hashCode = 1938039292;
                hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Id);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FirstName);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LastName);
                return hashCode;
            }

            //public bool Equals(int other)
            //{
            //    return other == this.Id;
            //}
        }
    }
}
