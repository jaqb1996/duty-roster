using RosterLibrary.Models;
using RosterLibrary;
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
using System.Text.RegularExpressions;
using RosterLibrary.DataAccess.CSV;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for CreateEmployeeWindow.xaml
    /// </summary>
    public partial class CreateEmployeeWindow : Window, IDataRequestor
    {
        List<IWorkingOption> availableOptions = new List<IWorkingOption>();
        ObservableCollection<IWorkingOption> addedOptions = new ObservableCollection<IWorkingOption>();
        IDataRequestor dataRequestor;
        public CreateEmployeeWindow(IDataRequestor requestor)
        {
            InitializeComponent();

            dataRequestor = requestor;
            OptionsForEmployeeListBox.ItemsSource = addedOptions;
            RefreshAvailableOptions();
        }
        public void DataReady()
        {
            RefreshAvailableOptions();
        }

        private void RefreshAvailableOptions()
        {
            // Refresh available options
            try
            {
                availableOptions = GlobalAccess.DataAccess.ReadAvailableWorkingOptions();
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
            }
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
            string firstName = EmployeeFirstName.Text;
            string lastName = EmployeeLastName.Text;
            int employeeID;
            try
            {
                // Add employee to database
                employeeID = GlobalAccess.DataAccess.AddEmployee(firstName, lastName);
            }
            catch (ArgumentException)
            {
                // Validation Unsuccessful
                MessageBox.Show("Podaj imię oraz nazwisko pracownika w odpowiednim formacie", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
                return;
            }
            // Connect chosen working options with employee 
            foreach (var option in addedOptions)
            {
                try
                {
                    GlobalAccess.DataAccess.AddWorkingOptionToEmployee(employeeID, option.Id);
                }
                catch (Exception)
                {
                    Helpers.ShowGeneralError();
                }
            }
            // Inform dataRequestor(ChooseEmployeeWindow) that data is ready
            dataRequestor.DataReady();
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
            CreateWorkingOptionWindow window = new CreateWorkingOptionWindow(this);
            window.ShowDialog();
        }

        
    }
}
