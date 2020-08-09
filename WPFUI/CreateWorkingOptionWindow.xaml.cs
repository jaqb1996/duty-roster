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
    /// Interaction logic for CreateWorkingOptionWindow.xaml
    /// </summary>
    public partial class CreateWorkingOptionWindow : Window
    {
        public CreateWorkingOptionWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string symbol = SymbolTextBox.Text;
            try
            {
                DateTime startingHour = new DateTime(1, 1, 1, int.Parse(StartingHourTextBox.Text), int.Parse(StartingMinuteTextBox.Text), 0);
                TimeSpan workingTime = new TimeSpan(int.Parse(WorkingTimeHourTextBox.Text), int.Parse(WorkingTimeMinuteTextBox.Text), 0);
                AppResources.DataAccess.AddWorkingOption(symbol, startingHour, workingTime);
            }
            catch (FormatException)
            {
                Helpers.ShowFormatError();
                return;
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
            }
            Close();
        }
    }
}
