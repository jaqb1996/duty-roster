using RosterLibrary.Calculations;
using RosterLibrary;
using RosterLibrary.Models;
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
    /// Interaction logic for NumberCalculationWindow.xaml
    /// </summary>
    public partial class NumberCalculationWindow : Window
    {
        readonly List<EmployeeDisplayData> rows = new List<EmployeeDisplayData>();
        public NumberCalculationWindow(ICalculatorReturningNumberForEmployee calculator)
        {
            InitializeComponent();

            // Fill header text and column header
            HeaderTextBlock.Text = $"Wynik obliczeń dla: {calculator.Name}";
            ResultDataGrid.Columns[2].Header = calculator.ResultName;

            // Make calculation
            Dictionary<IEmployeePresentationData, double> results = calculator.Calculate(GlobalAccess.Schedule);


            // Prepare list of employeeDisplayData
            foreach (var pair in results)
            {
                rows.Add(new EmployeeDisplayData() { FirstName = pair.Key.FirstName, LastName = pair.Key.LastName, Result = pair.Value.ToString() });
            }

            // Fill DataGrid with results
            ResultDataGrid.ItemsSource = rows;
        }
        public class EmployeeDisplayData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Result { get; set; }

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
