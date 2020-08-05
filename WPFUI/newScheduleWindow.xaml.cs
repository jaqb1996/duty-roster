using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            if (FirstDayPicker.SelectedDate == null || LastDayPicker.SelectedDate == null || name == "")
            {
                MessageBox.Show("Wybierz nazwę oraz pierwszy i ostatni dzień grafiku", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime startingDay = (DateTime)FirstDayPicker.SelectedDate;
            DateTime lastDay = (DateTime)LastDayPicker.SelectedDate;

            int id = AppResources.DataAccess.CreateSchedule(name, startingDay, lastDay);
            AppResources.Schedule = AppResources.DataAccess.LoadSchedule(id);

            ((MainWindow)Application.Current.MainWindow).RefreshSchedule();
            Close();

        }
    }
}
