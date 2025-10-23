using PerformPlus.Models;
using PerformPlus.Services;
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




namespace PerformPlus.Views
{
    public partial class OvertimeEntryDialog : Window
    {
        public OvertimeEntry Entry { get; set; }

        public OvertimeEntryDialog()
        {
            InitializeComponent();
            Entry = new OvertimeEntry
            {
                EntryDate = DateTime.Today,
                EmployeeID = SessionManager.EmployeeID 
            };
            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            OvertimeService.AddEntry(Entry);
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
        }
    }
}