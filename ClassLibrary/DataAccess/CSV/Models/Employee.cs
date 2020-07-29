using ClassLibrary.Models;

namespace ClassLibrary.DataAccess.CSV.Models
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
