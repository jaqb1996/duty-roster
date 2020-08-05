using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CreateEmployeeWindow.xaml
    /// </summary>
    public partial class CreateEmployeeWindow : Window
    {
        List<IWorkingOption> availableOptions = new List<IWorkingOption>();
        ObservableCollection<IWorkingOption> addedOptions = new ObservableCollection<IWorkingOption>();
        public CreateEmployeeWindow()
        {
            InitializeComponent();

            OptionsForEmployeeListBox.ItemsSource = addedOptions;
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            // Refresh available options on Window_Activated event to reflect changes done by other windows
            availableOptions = AppResources.DataAccess.ReadAvailableWorkingOptions();
            AvailableOptionsComboBox.ItemsSource = availableOptions;
        }
        private void AddOptionButton_Click(object sender, RoutedEventArgs e)
        {
            // Do not add option if no item is selected or item has been already added
            if (AvailableOptionsComboBox.SelectedItem is IWorkingOption newOption && !addedOptions.Contains(newOption))
            {
                addedOptions.Add(newOption);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate first and last name
            string firstName = EmployeeFirstName.Text;
            string lastName = EmployeeLastName.Text;
            if (firstName == "" || lastName == "")
            {
                MessageBox.Show("Podaj imię oraz nazwisko pracownika", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Add employee to database
            int employeeID = AppResources.DataAccess.AddEmployee(firstName, lastName);
            // Connect chosen working options with employee 
            foreach (var option in addedOptions)
            {
                AppResources.DataAccess.AddWorkingOptionToEmployee(employeeID, option.Id);
            }

            Close();
        }

        private void DeleteOptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsForEmployeeListBox.SelectedItem is IWorkingOption option)
            {
                addedOptions.Remove(option);
            }
        }

        private void NewOptionButton_Click(object sender, RoutedEventArgs e)
        {
            CreateWorkingOptionWindow window = new CreateWorkingOptionWindow();
            window.ShowDialog();
        }

        
    }
}
