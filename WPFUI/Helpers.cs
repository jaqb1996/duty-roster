using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using ClassLibrary.DataAccess.CSV;

namespace WPFUI
{
    static class Helpers
    {        
        public static (string symbol, DateTime startingHour, TimeSpan workingTime) WorkingOptionData(string symbol, string StartingHourString, string WorkingTimeString)
        {
            // Symbol validation
            if (!Regex.IsMatch(symbol, CSVDataAccess.SymbolPattern))
            {
                throw new ArgumentException("Symbol incorrect", "SymbolTextBox");
            }
            // Starting Hour validation
            if (!DateTime.TryParseExact(StartingHourString, WorkingOptionModel.StartingHourFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startingHour))
            {
                throw new ArgumentException("Incorrect format of starting hour", "StartingHourTextBox");
            }
            // Working Time validation
            if (!TimeSpan.TryParseExact(WorkingTimeString, WorkingOptionModel.WorkingTimeFormat, CultureInfo.InvariantCulture, out TimeSpan workingTime))
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
                    message = "Podaj symbol planu we właściwym formacie";
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
        public static readonly string firstNamePropertyName = "firstName";
        public static readonly string lastNamePropertyName = "lastName";
        public static void ShowGeneralError()
        {
            MessageBox.Show("Wystąpił błąd podczas wykonywania polecenia", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
