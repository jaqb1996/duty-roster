using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public interface ISchedule : ISchedulePresentationData
    {
        List<IEmployee> Employees { get; }
        //List<DateTime> WorkingDays { get; }
    }
}
