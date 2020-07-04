using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Calculations
{
    public interface ICalculatorReturningListOfNumbers : ICalculator
    {
        Dictionary<IEmployeePresentationData, List<double>> Calculate(ISchedule schedule);
    }
}
