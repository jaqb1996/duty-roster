using ClassLibrary.Models;
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
    /// Interaction logic for ChooseEmployeeWindow.xaml
    /// </summary>
    public partial class ChooseEmployeeWindow : Window
    {
        public ChooseEmployeeWindow()
        {
            InitializeComponent();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            // Refresh list on Window_Activated event to reflect changes done by other windows
            RefreshListOfEmployees();
        }
        public void RefreshListOfEmployees()
        {
            List<IEmployeePresentationData> employees = ((MainWindow)Application.Current.MainWindow).DataAccess.ReadNamesOfAvailableEmployees();
            availableEmployeesListbox.ItemsSource = employees;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var emp in availableEmployeesListbox.SelectedItems)
            {
                int employeeID = ((IEmployeePresentationData)emp).Id;
                int scheduleID = ((MainWindow)Application.Current.MainWindow).Schedule.Id;
                ((MainWindow)Application.Current.MainWindow).DataAccess.AddEmployeeToSchedule(scheduleID, employeeID);
                ((MainWindow)Application.Current.MainWindow).Schedule = ((MainWindow)Application.Current.MainWindow).DataAccess.LoadSchedule(scheduleID);
                ((MainWindow)Application.Current.MainWindow).RefreshSchedule();
            }
            Close();
        }

        private void NewEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            CreateEmployeeWindow window = new CreateEmployeeWindow();
            window.Show();
        }

        
    }
}
