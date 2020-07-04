using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DataAccess.CSV.Models
{
    class WorkingPlan
    {
        public int ScheduleID { get; set; }
        public int EmployeeID { get; set; }
        public List<IWorkingOption> WorkingOptions { get; set; }
    }
}
