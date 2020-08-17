using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosterLibrary.Calculations
{
    public interface ICalculatorReturningNumberForEmployee : ICalculator
    {
        Dictionary<IEmployeePresentationData, double> Calculate(ISchedule schedule);
    }
}
