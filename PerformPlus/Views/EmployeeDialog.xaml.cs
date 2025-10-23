using PerformPlus.Models;
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
using PerformPlus.ViewModels;
using PerformPlus.Views;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PerformPlus.Views
{
    public partial class EmployeeDialog : Window
    {
        public EmployeeModel Employee { get; private set; }

        public EmployeeDialog(EmployeeModel employee = null)
        {
            InitializeComponent();
            Employee = employee ?? new EmployeeModel { HireDate = DateTime.Now };
            DataContext = Employee;
        }

       
        private bool HasValidationErrors()
        {
            
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(Employee.Email) || !Regex.IsMatch(Employee.Email, emailPattern))
            {
                return true;
            }

           
            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d).+$";
            if (string.IsNullOrWhiteSpace(Employee.PasswordHash) || !Regex.IsMatch(Employee.PasswordHash, passwordPattern))
            {
                return true;
            }

            return false;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(Employee.Email) || !Regex.IsMatch(Employee.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                string emailError = (string)Application.Current.Resources["InvalidEmailMessage"];
                MessageBox.Show(emailError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Employee.PasswordHash) || !Regex.IsMatch(Employee.PasswordHash, @"^(?=.*[A-Z])(?=.*\d).+$"))
            {
                string passwordError = (string)Application.Current.Resources["InvalidPasswordMessage"];
                MessageBox.Show(passwordError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}