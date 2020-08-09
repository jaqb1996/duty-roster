﻿using ClassLibrary.Models;
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
            List<IEmployeePresentationData> employees;
            try
            {
                employees = AppResources.DataAccess.ReadNamesOfAvailableEmployees();
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
                return;
            }
            availableEmployeesListbox.ItemsSource = employees;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (availableEmployeesListbox.SelectedItem == null)
            {
                MessageBox.Show("Wybierz najpierw pracownika", "Brak wyboru", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int scheduleID = AppResources.Schedule.Id;
            bool atLeastOneAddded = false;
            foreach (object emp in availableEmployeesListbox.SelectedItems)
            {
                int employeeID = ((IEmployeePresentationData)emp).Id;
                try
                {
                    AppResources.DataAccess.AddEmployeeToSchedule(scheduleID, employeeID);
                    atLeastOneAddded = true;
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Pracownik został już dodany", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);

                    // Dont't return, load schedule and close window to show already added employees
                    // Break the loop because collection of selected items changes with OK press 
                    break;
                    
                }
                catch (Exception)
                {
                    Helpers.ShowGeneralError(); 
                }
            }
            try
            {
                if (atLeastOneAddded)
                {
                    MessageBox.Show("Refreshing");
                    AppResources.Schedule = AppResources.DataAccess.LoadSchedule(scheduleID);
                    ((MainWindow)Application.Current.MainWindow).RefreshSchedule();
                    Close();
                }
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
                return;

            }
            
        }

        private void NewEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            CreateEmployeeWindow window = new CreateEmployeeWindow();
            window.Show();
        }

        
    }
}
