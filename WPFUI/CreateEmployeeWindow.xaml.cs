﻿using RosterLibrary.Models;
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
            try
            {
                availableOptions = AppResources.DataAccess.ReadAvailableWorkingOptions();
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
                employeeID = AppResources.DataAccess.AddEmployee(firstName, lastName);
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
                    AppResources.DataAccess.AddWorkingOptionToEmployee(employeeID, option.Id);
                }
                catch (Exception)
                {
                    Helpers.ShowGeneralError();
                }
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
