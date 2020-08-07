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
            string symbol;
            DateTime startingHour;
            TimeSpan workingTime;
            try
            {
                (symbol, startingHour, workingTime) = Helpers.WorkingOptionData(SymbolTextBox.Text, StartingHourTextBox.Text, WorkingTimeTextBox.Text);
            }
            catch (ArgumentException ex)
            {
                Helpers.DisplayWorkingOptionError(ex);
                return;
            }
            try
            {
                AppResources.DataAccess.AddWorkingOption(symbol, startingHour, workingTime);
            }
            catch (Exception)
            {
                Helpers.ShowGeneralError();
            }
            Close();
        }
    }
}
