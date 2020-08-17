using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosterLibrary.Calculations.Helpers
{
    public class HolidayChecker
    {
        public static bool IsHoliday(DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            switch (dt.Month)
            {
                case 1:
                    {
                        if (dt.Day == 1 || dt.Day == 6)
                        {
                            return true;
                        }
                        break;
                    }
                case 5:
                    {
                        if (dt.Day == 1 || dt.Day == 3)
                        {
                            return true;
                        }
                        break;
                    }
                case 8:
                    {
                        if (dt.Day == 15)
                        {
                            return true;
                        }
                        break;
                    }
                case 11:
                    {
                        if (dt.Day == 1 || dt.Day == 11)
                        {
                            return true;
                        }
                        break;
                    }
                case 12:
                    {
                        if (dt.Day == 25 || dt.Day == 26)
                        {
                            return true;
                        }
                        break;
                    }
            }

            if (dt.Month >= 3 || dt.Month <= 6)
            {
                //obliczanie Wielkanocy
                int a = dt.Year % 19;
                int b = (int)Math.Floor((decimal)(dt.Year / 100));
                int c = dt.Year % 100;
                int d = (int)Math.Floor((decimal)(b / 4));
                int e = b % 4;
                int f = (int)Math.Floor((decimal)((b + 8) / 25));
                int g = (int)Math.Floor((decimal)((b - f + 1) / 3));
                int h = (19 * a + b - d - g + 15) % 30;
                int i = (int)Math.Floor((decimal)(c / 4));
                int k = c % 4;
                int l = (32 + 2 * e + 2 * i - h - k) % 7;
                int m = (int)Math.Floor((decimal)((a + 11 * h + 22 * l) / 451));
                int p = (h + l - 7 * m + 114) % 31;
                int day = p + 1;
                int month = (int)Math.Floor((decimal)((h + l - 7 * m + 114) / 31));

                DateTime Easter = new DateTime(dt.Year, month, day);

                if (dt == Easter || dt == Easter.AddDays(1) || dt == Easter.AddDays(49) || dt == Easter.AddDays(60))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
