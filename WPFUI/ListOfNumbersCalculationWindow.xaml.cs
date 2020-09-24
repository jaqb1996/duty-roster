using RosterLibrary.Calculations;
using RosterLibrary.Models;
using RosterLibrary;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
    /// Interaction logic for ListOfNumbersCalculationWindow.xaml
    /// </summary>
    public partial class ListOfNumbersCalculationWindow : Window
    {
        private readonly string numberPropertyNameCommonPart = "Result";
        private readonly string columnHeaderCommonPart;
        public ListOfNumbersCalculationWindow(ICalculatorReturningListOfNumbers calculator)
        {
            InitializeComponent();

            // Fill header text and result column header
            HeaderTextBlock.Text = $"Wynik obliczeń dla: {calculator.Name}";
            columnHeaderCommonPart = calculator.ResultName;

            // Make calculation
            Dictionary<IEmployeePresentationData, List<double>> results = calculator.Calculate(GlobalAccess.Schedule);

            // Generate columns for last and first name 
            DataGridTextColumn lastNameColumn = new DataGridTextColumn
            {
                Header = "Nazwisko",
                Binding = new Binding(Helpers.lastNamePropertyName),
                IsReadOnly = true
            };
            ResultDataGrid.Columns.Add(lastNameColumn);

            DataGridTextColumn firstNameColumn = new DataGridTextColumn
            {
                Header = "Imię",
                Binding = new Binding(Helpers.firstNamePropertyName),
                IsReadOnly = true
            };
            ResultDataGrid.Columns.Add(firstNameColumn);

            // Add as many columns as many numbers were generated for each employee
            int numberOfResults = results.Values.First().Count;
            for (int i = 0; i < numberOfResults; i++)
            {
                DataGridTextColumn column = new DataGridTextColumn
                {
                    Header = $"{columnHeaderCommonPart} {i + 1}",
                    Binding = new Binding(GetPropertyName(i)),
                    IsReadOnly = true
                };
                ResultDataGrid.Columns.Add(column);
            }
            // ExpandoObject is used because of variable number of results in different schedules
            List<ExpandoObject> rows = new List<ExpandoObject>();

            // Generate row for each employee
            foreach (IEmployee employee in results.Keys)
            {
                dynamic row = new ExpandoObject();
                ((IDictionary<string, object>)row).Add(Helpers.lastNamePropertyName, employee.LastName);
                ((IDictionary<string, object>)row).Add(Helpers.firstNamePropertyName, employee.FirstName);

                // Generate as many fiels as many results
                for (int i = 0; i < numberOfResults; i++)
                {
                    ((IDictionary<string, object>)row).Add(GetPropertyName(i), results[employee][i]);
                }

                rows.Add(row);
            }

            ResultDataGrid.ItemsSource = rows;

        }
        private string GetPropertyName(int i) => numberPropertyNameCommonPart + i;

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
