using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosterLibrary.Calculations
{
    public interface ICalculatorReturningListOfNumbers : ICalculator
    {
        Dictionary<IEmployeePresentationData, List<double>> Calculate(ISchedule schedule);
    }
}
