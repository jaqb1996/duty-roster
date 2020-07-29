using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFUI
{
    class Helpers
    {
        public static (string symbol, DateTime startingHour, TimeSpan workingTime) WorkingOptionData(string symbol, string StartingHourString, string WorkingTimeString)
        {
            // Symbol validation
            if (symbol == "")
            {
                throw new ArgumentException("Symbol incorrect", "SymbolTextBox");
            }
            // Starting Hour validation
            if (!DateTime.TryParseExact(StartingHourString, "hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startingHour))
            {
                throw new ArgumentException("Incorrect format of starting hour", "StartingHourTextBox");
            }
            // Working Time validation
            if (!TimeSpan.TryParseExact(WorkingTimeString, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan workingTime))
            {
                throw new ArgumentException("Incorrect format of starting hour", "WorkingTimeTextBox");
            }
            return (symbol, startingHour, workingTime);
        }
        public static void DisplayWorkingOptionError(ArgumentException ex)
        {
            string message;
            switch (ex.ParamName)
            {
                case "SymbolTextBox":
                    message = "Podaj symbol planu";
                    break;
                case "StartingHourTextBox":
                    message = "Podaj czas rozpoczęcia pracy we właściwym formacie (hh:mm)";
                    break;
                case "WorkingTimeTextBox":
                    message = "Podaj czas pracy we właściwym formacie (hh:mm)";
                    break;
                default:
                    throw new Exception("No parameter match");
            }
            MessageBox.Show(message, "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
