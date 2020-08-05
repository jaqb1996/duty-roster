﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Calculations
{
    public interface ICalculator
    {
        string Name { get; }
        string Description { get; }
        string ResultName { get; }
    }
}
