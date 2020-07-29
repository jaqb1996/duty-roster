using ClassLibrary.DataAccess;
using ClassLibrary.DataAccess.CSV;
using ClassLibrary.Models;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // Let user choose schedule as soon as window loads 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChooseScheduleWindow window = new ChooseScheduleWindow();
            window.ShowDialog();
        }
        public IDataAccess DataAccess = new CSVDataAccess();
        public ISchedule Schedule { get; set; } = null;

        public bool Modified = false;

        private readonly string dateHeaderFormat = "%d.%M.%y";
        private readonly string datePropertyFormat = "%d_%M_%y";

        
        /// <summary>
        /// Generates view of Schedule
        /// </summary>
        public void RefreshSchedule()
        {
            if (Schedule != null)
            {
                mainGrid.Columns.Clear();

                GenerateColumns();

                PopulateRows();

                mainGrid.ItemsSource = rows;
                
                scheduleTitle.Text = $"Grafik \"{Schedule.Name}\"";

            }
        }
        /// <summary>
        /// Prepares DataGrid columns headers and bindings 
        /// </summary>
        private void GenerateColumns()
        {
            DataGridTextColumn lastNameColumn = new DataGridTextColumn
            {
                Header = "Nazwisko",
                Binding = new Binding(lastNamePropertyName),
                IsReadOnly = true
            };
            mainGrid.Columns.Add(lastNameColumn);

            DataGridTextColumn firstNameColumn = new DataGridTextColumn
            {
                Header = "Imię",
                Binding = new Binding(firstNamePropertyName),
                IsReadOnly = true
            };
            mainGrid.Columns.Add(firstNameColumn);

            Schedule.IterateOverAllDays((day) =>
            {
                DataGridTextColumn column = new DataGridTextColumn
                {
                    Header = day.ToString(dateHeaderFormat),
                    Binding = new Binding(GetDatePropertyName(day))
                };
                mainGrid.Columns.Add(column);
            });
        }
        // ExpandoObject is used because of variable number of days in different schedules
        private List<ExpandoObject> rows;

        private readonly string firstNamePropertyName = "firstName";
        private readonly string lastNamePropertyName = "lastName";
        private readonly string idPropertyName = "Id";
        private void PopulateRows()
        {
            rows = new List<ExpandoObject>();
            // Generate row for each employee
            foreach (IEmployee employee in Schedule.Employees)
            {
                dynamic row = new ExpandoObject();
                ((IDictionary<string, object>)row).Add(idPropertyName, employee.Id);
                ((IDictionary<string, object>)row).Add(lastNamePropertyName, employee.LastName);
                ((IDictionary<string, object>)row).Add(firstNamePropertyName, employee.FirstName);
                int i = 0;
                // Generate as many fiels as schedule has days
                Schedule.IterateOverAllDays((day) =>
                {
                    ((IDictionary<string, object>)row).Add(GetDatePropertyName(day), employee.WorkingPlan[i].Symbol);
                    i++;
                });
                rows.Add(row);
            }
        }


        private void chooseScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseScheduleWindow window = new ChooseScheduleWindow();
            window.ShowDialog();
        }
        

        
        private string GetDatePropertyName(DateTime day)
        {
            return $"date{day.ToString(datePropertyFormat)}";
        }
        

        private void newEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Schedule == null)
            {
                MessageBox.Show("Najpierw załaduj grafik", "Brak wybranego grafika", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ChooseEmployeeWindow window = new ChooseEmployeeWindow();
            window.Show();
        }


        private void mainGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            string dateString = e.Column.Header.ToString();
            DateTime date = DateTime.Parse(dateString);
            IDictionary<string, object> row = e.Row.Item as IDictionary<string, object>;
            EditPlanForDayWindow window = new EditPlanForDayWindow(row[firstNamePropertyName].ToString(),
                                                                   row[lastNamePropertyName].ToString(),
                                                                   (int)row[idPropertyName],
                                                                   date);
            window.ShowDialog();
            e.Cancel = true;
        }

        private void saveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Modified)
            {
                DataAccess.SaveSchedule(Schedule);
                Modified = false;
                Title = "Grafik";
            }
        }
    }
}
