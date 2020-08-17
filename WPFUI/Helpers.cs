using RosterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using RosterLibrary.DataAccess.CSV;

namespace WPFUI
{
    static class Helpers
    {        
        public static readonly string firstNamePropertyName = "firstName";
        public static readonly string lastNamePropertyName = "lastName";
        public static void ShowGeneralError(Exception ex = null)
        {
            MessageBox.Show($"Wystąpił błąd podczas wykonywania polecenia", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void ShowFormatError()
        {
            MessageBox.Show("Wprowadź dane we właściwym formacie", "Niewłaściwy format danych", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void LoadAndRefreshSchedule(int id)
        {
            try
            {
                AppResources.Schedule = AppResources.DataAccess.LoadSchedule(id);
            }
            catch (Exception ex)
            {
                Helpers.ShowGeneralError(ex);
                return;
            }
            ((MainWindow)Application.Current.MainWindow).RefreshSchedule();
        }
    }
}
