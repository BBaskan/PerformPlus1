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
using PerformPlus.Services;
using PerformPlus.Models;
using PerformPlus.ViewModels;
using PerformPlus.Views;




namespace PerformPlus.Views
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
           
            DataContext = new AdminViewModel();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            
            new Login().Show();
            this.Close();
        }
    }
}