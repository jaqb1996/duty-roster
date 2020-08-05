using ClassLibrary.Calculations;
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
    /// Interaction logic for ChooseCalculatorWindow.xaml
    /// </summary>
    public partial class ChooseCalculatorWindow : Window
    {
        // TODO: Automatically find all available calculators
        private readonly ICalculator[] calculators = new ICalculator[] { new HolidayNightHoursCalculator(), new WeekHoursCalculator() };
        public ChooseCalculatorWindow()
        {
            InitializeComponent();

            AvailableCalculatorsComboBox.ItemsSource = calculators;
        }

        private void AvailableCalculatorsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DescriptionTextBox.Text = (AvailableCalculatorsComboBox.SelectedItem as ICalculator).Description;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableCalculatorsComboBox.SelectedItem == null)
                return;

            // start new kind of form depending on type of calculator chosen
            if (AvailableCalculatorsComboBox.SelectedItem is ICalculatorReturningListOfNumbers calculator)
            {
                ListOfNumbersCalculationWindow window = new ListOfNumbersCalculationWindow(calculator);
                window.Show();
            }
            else if (AvailableCalculatorsComboBox.SelectedItem is ICalculatorReturningNumberForEmployee calculatorReturningNumber)
            {
                NumberCalculationWindow window = new NumberCalculationWindow(calculatorReturningNumber);
                window.Show();
            }
            Close();
        }
    }
}
