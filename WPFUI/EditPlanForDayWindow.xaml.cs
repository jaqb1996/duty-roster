using RosterLibrary.Models;
using RosterLibrary;
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
            IWorkingOption plan = GlobalAccess.Schedule.PlanForDay(employeeId, date);
            string symbol = plan.Symbol;
            if (symbol != WorkingOptionModel.DefaultSymbol)
            {
                SymbolTextBox.Text = symbol;
                FillTimeFields(plan);
            }

            this.date = date;
            this.employeeId = employeeId;
        }

        private void FillTimeFields(IWorkingOption plan)
        {
            // Format numbers with leading zero
            StartingHourTextBox.Text = plan.StartingHour.ToString("HH");
            StartingMinuteTextBox.Text = plan.StartingHour.ToString("mm");

            WorkingTimeHourTextBox.Text = plan.WorkingTime.ToString("hh");
            WorkingTimeMinuteTextBox.Text = plan.WorkingTime.ToString("mm");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ModifySchedule();
        }

        private void ModifySchedule()
        {
            string symbol = SymbolTextBox.Text;
            try
            {
                DateTime startingHour = new DateTime(1, 1, 1, int.Parse(StartingHourTextBox.Text), int.Parse(StartingMinuteTextBox.Text), 0);
                TimeSpan workingTime = new TimeSpan(int.Parse(WorkingTimeHourTextBox.Text), int.Parse(WorkingTimeMinuteTextBox.Text), 0);
                GlobalAccess.Schedule.ChangeWorkDay(employeeId, date, symbol, startingHour, workingTime);
            }
            catch (FormatException)
            {
                Helpers.ShowFormatError();
                return;
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
            }
                    ((MainWindow)Application.Current.MainWindow).RefreshSchedule();
            SendInformationAboutModification();
            Close();
        }

        private void SendInformationAboutModification()
        {
            ((MainWindow)Application.Current.MainWindow).Modified = true;
            ((MainWindow)Application.Current.MainWindow).Title = "Grafik *";
        }
        private void SymbolTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SymbolTextBox.Text == "")
                return;
            // Fill starting hour and working time if employee has default working option with chosen symbol 
            IEmployee employee = (from emp in GlobalAccess.Schedule.Employees where emp.Id == employeeId select emp).Single();

            foreach (IWorkingOption option in employee.AvailableOptions)
            {
                if (option.Symbol == SymbolTextBox.Text)
                {
                    FillTimeFields(option);
                }
                
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ModifySchedule();
        }
    }
}
