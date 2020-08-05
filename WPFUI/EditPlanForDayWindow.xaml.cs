using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for EditPlanForDayWindow.xaml
    /// </summary>
    public partial class EditPlanForDayWindow : Window
    {
        int employeeId;
        DateTime date;
        public EditPlanForDayWindow(string employeeFirstName, string employeeLastName, int employeeId, DateTime date)
        {
            InitializeComponent();

            // Fill the header with employee and date information
            EmployeeNameTextBlock.Text = $"{employeeFirstName} {employeeLastName}";
            DateTextBlock.Text = date.ToString("dd.MM.yyyy");

            // Fill fields with current values if already modified
            IWorkingOption plan = AppResources.Schedule.PlanForDay(employeeId, date);
            string symbol = plan.Symbol;
            if (symbol != "")
            {
                SymbolTextBox.Text = symbol;
                StartingHourTextBox.Text = plan.StartingHour.ToString("hh:mm");
                WorkingTimeTextBox.Text = plan.WorkingTime.ToString(@"hh\:mm");
            }

            this.date = date;
            this.employeeId = employeeId;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            DateTime startingHour;
            TimeSpan workingTime;
            try
            {
                (symbol, startingHour, workingTime) = Helpers.WorkingOptionData(SymbolTextBox.Text, StartingHourTextBox.Text, WorkingTimeTextBox.Text);

            }
            catch (ArgumentException ex)
            {
                Helpers.DisplayWorkingOptionError(ex);
                return;
            }
            AppResources.Schedule.ChangeWorkDay(employeeId, date, symbol, startingHour, workingTime);
            ((MainWindow)Application.Current.MainWindow).RefreshSchedule();
            sendInformationAboutModification();
            Close();
        }
        private void sendInformationAboutModification()
        {
            ((MainWindow)Application.Current.MainWindow).Modified = true;
            ((MainWindow)Application.Current.MainWindow).Title = "Grafik (zmodyfikowany)";
        }
        private void SymbolTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SymbolTextBox.Text == "")
                return;
            // Fill starting hour and working time if employee has default working option with chosen symbol 
            IEmployee employee = (from emp in AppResources.Schedule.Employees where emp.Id == employeeId select emp).Single();
            foreach (IWorkingOption option in employee.AvailableOptions)
            {
                if (option.Symbol == SymbolTextBox.Text)
                {
                    StartingHourTextBox.Text = option.StartingHour.ToString("hh:mm");
                    WorkingTimeTextBox.Text = option.WorkingTime.ToString(@"hh\:mm");
                }
            }
        }
    }
}
