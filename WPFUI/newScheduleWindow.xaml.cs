using RosterLibrary.DataAccess.CSV;
using RosterLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for newScheduleWindow.xaml
    /// </summary>
    public partial class newScheduleWindow : Window
    {
        public newScheduleWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NewScheduleName.Text;
            if (FirstDayPicker.SelectedDate == null || LastDayPicker.SelectedDate == null || !Regex.IsMatch(name, CSVDataAccess.NamePattern))
            {
                MessageBox.Show("Wybierz nazwę we właściwym formacie oraz pierwszy i ostatni dzień grafiku", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime startingDay = (DateTime)FirstDayPicker.SelectedDate;
            DateTime lastDay = (DateTime)LastDayPicker.SelectedDate;
            int id;
            try
            {
                id = GlobalAccess.DataAccess.CreateSchedule(name, startingDay, lastDay);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Pierwszy dzień grafiku musi być przed ostatnim dniem", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Helpers.LoadAndRefreshSchedule(id);
            Close();

        }
    }
}
