using RosterLibrary.Calculations.Helpers;
using RosterLibrary;
using RosterLibrary.Models;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }
        // Let user choose schedule as soon as window loads 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChooseScheduleWindow window = new ChooseScheduleWindow();
            window.ShowDialog();
        }

        // Notify SaveChangesButton that Modified property (bound to the button's IsEnabled) has changed
        public event PropertyChangedEventHandler PropertyChanged;
        private bool modified = false;
        public bool Modified 
        {
            get => modified;
            set
            {
                if (value != modified)
                {
                    modified = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Modified"));
                }
            } 
        }

        private readonly string dateHeaderFormat = "%d.%M.%y";
        private readonly string datePropertyFormat = "%d_%M_%y";

        
        /// <summary>
        /// Generates view of Schedule
        /// </summary>
        public void RefreshSchedule()
        {
            if (GlobalAccess.Schedule != null)
            {
                mainGrid.Columns.Clear();

                GenerateColumns();

                PopulateRows();

                mainGrid.ItemsSource = rows;
                
                scheduleTitle.Text = $"Grafik \"{GlobalAccess.Schedule.Name}\"";

            }
        }
        
        private readonly string idPropertyName = "Id";
        /// <summary>
        /// Prepares DataGrid columns headers and bindings 
        /// </summary>
        private void GenerateColumns()
        {
            DataGridTextColumn lastNameColumn = new DataGridTextColumn
            {
                Header = "Nazwisko",
                Binding = new Binding(Helpers.lastNamePropertyName),
                IsReadOnly = true
            };
            mainGrid.Columns.Add(lastNameColumn);
            DataGridTextColumn firstNameColumn = new DataGridTextColumn
            {
                Header = "Imię",
                Binding = new Binding(Helpers.firstNamePropertyName),
                IsReadOnly = true
            };
            mainGrid.Columns.Add(firstNameColumn);

            GlobalAccess.Schedule.IterateOverAllDays((day) =>
            {
                DataGridTextColumn column = new DataGridTextColumn
                {
                    Header = day.ToString(dateHeaderFormat),
                    Binding = new Binding(GetDatePropertyName(day))
                };

                // Set different colour for holidays columns
                if (HolidayChecker.IsHoliday(day))
                {
                    Style style = (Style)mainGrid.FindResource("HolidayColumnStyle");
                    column.ElementStyle = style;
                }
                mainGrid.Columns.Add(column);
            });
        }
        // ExpandoObject is used because of possibility of variable number of days in different schedules
        private List<ExpandoObject> rows;


        private void PopulateRows()
        {
            rows = new List<ExpandoObject>();
            // Generate row for each employee
            foreach (IEmployee employee in GlobalAccess.Schedule.Employees)
            {
                dynamic row = new ExpandoObject();
                ((IDictionary<string, object>)row).Add(idPropertyName, employee.Id);
                ((IDictionary<string, object>)row).Add(Helpers.lastNamePropertyName, employee.LastName);
                ((IDictionary<string, object>)row).Add(Helpers.firstNamePropertyName, employee.FirstName);
                int i = 0;
                // Generate as many fiels as schedule has days
                GlobalAccess.Schedule.IterateOverAllDays((day) =>
                {
                    // Leave empty space for default value
                    ((IDictionary<string, object>)row).Add(GetDatePropertyName(day), employee.WorkingPlan[i].Symbol == "W" ? "" : employee.WorkingPlan[i].Symbol);
                    i++;
                });
                rows.Add(row);
            }
        }

        private void chooseScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChangesDialog(() =>
            {
                ChooseScheduleWindow window = new ChooseScheduleWindow();
                window.ShowDialog();
            }, () => { }); // Don't do anything on user's Cancel
            
        }
        
        private string GetDatePropertyName(DateTime day)
        {
            return $"date{day.ToString(datePropertyFormat)}";
        }

        private void newEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalAccess.Schedule == null)
            {
                MessageBox.Show("Najpierw załaduj grafik", "Brak wybranego grafika", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ChooseEmployeeWindow window = new ChooseEmployeeWindow();
            window.Show();
        }

        private void mainGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // Get necessary data for EditPlanForDayWindow and show it
            string dateString = e.Column.Header.ToString();
            DateTime date = DateTime.Parse(dateString);
            IDictionary<string, object> row = e.Row.Item as IDictionary<string, object>;
            EditPlanForDayWindow window = new EditPlanForDayWindow(row[Helpers.firstNamePropertyName].ToString(),
                                                                   row[Helpers.lastNamePropertyName].ToString(),
                                                                   (int)row[idPropertyName],
                                                                   date);
            window.ShowDialog();
            // Stop editting - schedule has been already edited in EditPlanForDayWindow
            e.Cancel = true;
        }

        private void saveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Modified)
            {
                try
                {
                    GlobalAccess.DataAccess.SaveSchedule(GlobalAccess.Schedule);
                    SetNoLongerModified();
                }
                catch (Exception)
                {
                    Helpers.ShowGeneralError();
                }
            }
        }

        private void SetNoLongerModified()
        {
            Modified = false;
            Title = "Grafik";
        }

        private void calculationsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckScheduleExistence())
                return;
            ChooseCalculatorWindow window = new ChooseCalculatorWindow();
            window.ShowDialog();
        }
        private bool CheckScheduleExistence()
        {
            if (GlobalAccess.Schedule == null)
            {
                MessageBox.Show("Najpierw załaduj grafik", "Brak wybranego grafika", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveChangesDialog(Application.Current.Shutdown, () => { e.Cancel = true; });
        }
        /// <summary>
        /// Asks user if he wants to save changes if schedule has been modified and then takes appropriate actions
        /// </summary>
        /// <param name="mainAction">What to do after user's No, after saving and if not modified</param>
        /// <param name="actionForCancel">What to do after user's Cancel</param>
        private void SaveChangesDialog(Action mainAction, Action actionForCancel)
        {
            if (Modified)
            {
                MessageBoxResult answer = MessageBox.Show("Czy chcesz zapisać zmiany?", "Zapisać zmiany?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (answer)
                {
                    case MessageBoxResult.Cancel:
                        actionForCancel.Invoke();
                        break;
                    case MessageBoxResult.Yes:
                        try
                        {
                            GlobalAccess.DataAccess.SaveSchedule(GlobalAccess.Schedule);
                            SetNoLongerModified();
                        }
                        catch (Exception)
                        {
                            Helpers.ShowGeneralError();
                        }
                        mainAction.Invoke();
                        break;
                    case MessageBoxResult.No:
                        mainAction.Invoke();
                        break;
                }
            }
            else
            {
                mainAction.Invoke();
            }
        }

        private void DeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckScheduleExistence())
                return;
            if (MessageBox.Show("Czy na pewno chcesz trwale usunąć grafik?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Warning) !=
                MessageBoxResult.Yes)
                return;
            try
            {
                GlobalAccess.DataAccess.DeleteSchedule(GlobalAccess.Schedule.Id);
                GlobalAccess.Schedule = null;
                // Clear columns to protect from using nonexistent schedule
                mainGrid.Columns.Clear();
                scheduleTitle.Text = "Nie wczytano grafiku";
            }
            catch (Exception ex)
            {
                Helpers.ShowGeneralError(ex);
                return;
            }
            ChooseScheduleWindow window = new ChooseScheduleWindow();
            window.ShowDialog();
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChangesDialog(DeleteEmployeeFromSchedule, 
                () => { }); // Don't do anything on user's Cancel
        }

        private void DeleteEmployeeFromSchedule()
        {
            if (!CheckScheduleExistence())
                return;
            int index = mainGrid.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("Musisz najpierw zaznaczyć wiersz odpowiadający pracownikowi", "Brak wyboru pracownika", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Get ExpandoObject bound to selected row
            var employee = (IDictionary<string, object>)rows[index];
            var employeeID = (int)employee[idPropertyName];
            var firstName = (string)employee[Helpers.firstNamePropertyName];
            var lastName = (string)employee[Helpers.lastNamePropertyName];

            // Make sure user wants to delete employee
            if (MessageBox.Show($"Czy na pewno chcesz usunąć pracownika {firstName} {lastName} z grafiku?", "Potwierdź usunięcie pracownika",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            // Delete employee
            try
            {
                int scheduleID = GlobalAccess.Schedule.Id;
                GlobalAccess.DataAccess.DeleteEmployeeFromSchedule(employeeID, scheduleID);
                Helpers.LoadAndRefreshSchedule(scheduleID);
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
                return;
            }
        }
    }
}
