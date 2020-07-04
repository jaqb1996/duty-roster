using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public interface IEmployeePresentationData : IEquatable<IEmployeePresentationData>
    {
        int Id { get; }
        string FirstName { get; }
        string LastName { get; }
    }
}
