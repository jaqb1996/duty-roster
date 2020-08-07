using ClassLibrary.DataAccess;
using ClassLibrary.DataAccess.CSV;
using ClassLibrary.Models;
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
            if (AppResources.Schedule != null)
            {
                mainGrid.Columns.Clear();

                GenerateColumns();

                PopulateRows();

                mainGrid.ItemsSource = rows;
                
                scheduleTitle.Text = $"Grafik \"{AppResources.Schedule.Name}\"";

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

            AppResources.Schedule.IterateOverAllDays((day) =>
            {
                DataGridTextColumn column = new DataGridTextColumn
                {
                    Header = day.ToString(dateHeaderFormat),
                    Binding = new Binding(GetDatePropertyName(day))
                };
                mainGrid.Columns.Add(column);
            });
        }
        // ExpandoObject is used because of possibility of variable number of days in different schedules
        private List<ExpandoObject> rows;


        private void PopulateRows()
        {
            rows = new List<ExpandoObject>();
            // Generate row for each employee
            foreach (IEmployee employee in AppResources.Schedule.Employees)
            {
                dynamic row = new ExpandoObject();
                ((IDictionary<string, object>)row).Add(idPropertyName, employee.Id);
                ((IDictionary<string, object>)row).Add(Helpers.lastNamePropertyName, employee.LastName);
                ((IDictionary<string, object>)row).Add(Helpers.firstNamePropertyName, employee.FirstName);
                int i = 0;
                // Generate as many fiels as schedule has days
                AppResources.Schedule.IterateOverAllDays((day) =>
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
            if (AppResources.Schedule == null)
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
                    AppResources.DataAccess.SaveSchedule(AppResources.Schedule);
                }
                catch (Exception)
                {
                    Helpers.ShowGeneralError();
                }
                Modified = false;
                Title = "Grafik";
            }
        }

        private void calculationsButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppResources.Schedule == null)
            {
                MessageBox.Show("Najpierw załaduj grafik", "Brak wybranego grafika", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ChooseCalculatorWindow window = new ChooseCalculatorWindow();
            window.ShowDialog();
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
                MessageBoxResult answer = MessageBox.Show("Czy chcesz zapisać zmiany?", "Wyjście", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (answer)
                {
                    case MessageBoxResult.Cancel:
                        actionForCancel.Invoke();
                        break;
                    case MessageBoxResult.Yes:
                        try
                        {
                            AppResources.DataAccess.SaveSchedule(AppResources.Schedule);
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
            
    }
}
