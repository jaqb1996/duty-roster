using RosterLibrary.Models;

namespace RosterLibrary.DataAccess.Models
{
    class Employee : IEmployeePresentationData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool Equals(IEmployeePresentationData other)
        {
            return other != null &&
                   FirstName == other.FirstName &&
                   LastName == other.LastName;
        }
    }
}
