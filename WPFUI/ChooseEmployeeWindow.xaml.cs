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
    /// Interaction logic for ChooseEmployeeWindow.xaml
    /// </summary>
    public partial class ChooseEmployeeWindow : Window, IDataRequestor
    {
        public ChooseEmployeeWindow()
        {
            InitializeComponent();

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
        // Refresh list of employees when data filled in other windowa is ready
        public void DataReady()
        {
            RefreshListOfEmployees();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEmployeeSelected())
                return;
            int scheduleID = AppResources.Schedule.Id;
            bool atLeastOneAdded = false;
            foreach (object emp in availableEmployeesListbox.SelectedItems)
            {
                var employee = ((IEmployeePresentationData)emp);
                try
                {
                    AppResources.DataAccess.AddEmployeeToSchedule(scheduleID, employee.Id);
                    atLeastOneAdded = true;
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show($"Pracownik {employee.FirstName} {employee.LastName} został już dodany", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);

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
                if (atLeastOneAdded)
                {
                    Helpers.LoadAndRefreshSchedule(scheduleID);
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
            CreateEmployeeWindow window = new CreateEmployeeWindow(this);
            window.Show();
        }

        private void DeleteEmloyeesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEmployeeSelected())
                return;

            // Delete only the first one of selected employees
            IEmployeePresentationData emp = (IEmployeePresentationData)availableEmployeesListbox.SelectedItem;

            // Make sure user really wants to delete
            if (MessageBox.Show($"Czy na pewno chcesz usunąć pracownika {emp.FirstName} {emp.LastName} z bazy danych i wszystkich grafików?", "Potwierdź usunięcie", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            // Delete employee
            try
            {
                AppResources.DataAccess.DeleteEmployee(emp.Id);
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
                return;
            }
            RefreshListOfEmployees();
            Helpers.LoadAndRefreshSchedule(AppResources.Schedule.Id);
        }
        private bool CheckEmployeeSelected()
        {
            if (availableEmployeesListbox.SelectedItem == null)
            {
                MessageBox.Show("Wybierz najpierw pracownika", "Brak wyboru", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        
    }
}
