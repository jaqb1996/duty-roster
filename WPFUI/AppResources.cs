﻿using RosterLibrary.DataAccess;
using RosterLibrary.DataAccess.CSV;
using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI
{
    public static class AppResources
    {
        public static ISchedule Schedule { get; set; }
        public static IDataAccess DataAccess { get; }
        static AppResources()
        {
            Schedule = null;
            DataAccess = new CSVDataAccess();
        }

    }
}
