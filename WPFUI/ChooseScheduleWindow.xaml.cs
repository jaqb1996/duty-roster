using ClassLibrary.DataAccess;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ChooseScheduleWindow.xaml
    /// </summary>
    public partial class ChooseScheduleWindow : Window
    {
        public ChooseScheduleWindow()
        {   
            InitializeComponent();

            List<ISchedulePresentationData> schedules = ((MainWindow)Application.Current.MainWindow).DataAccess.ReadNamesOfAvailableSchedules();
            availableSchedulesListbox.ItemsSource = schedules;

        }

        private void openSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (availableSchedulesListbox.SelectedItem == null)
            {
                // TODO message box: you must choose schedule
                return;
            }
            int id = ((ISchedulePresentationData)availableSchedulesListbox.SelectedItem).Id;
            ((MainWindow)Application.Current.MainWindow).Schedule = ((MainWindow)Application.Current.MainWindow).DataAccess.LoadSchedule(id);
            //MessageBox.Show($"{((MainWindow)Application.Current.MainWindow).Schedule.Employees[0].WorkingPlan[0].Symbol}");
            ((MainWindow)Application.Current.MainWindow).RefreshSchedule();
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void newScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            newScheduleWindow window = new newScheduleWindow();
            window.ShowDialog();
            
        }
    }
}
