using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using PerformPlus.Services;
using PerformPlus.Models;
using PerformPlus.ViewModels;
using PerformPlus.Views;
using System.Windows;

namespace PerformPlus.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        
        public ObservableCollection<EmployeeModel> Employees { get; } = new ObservableCollection<EmployeeModel>();
        public ObservableCollection<PayrollModel> Payrolls { get; } = new ObservableCollection<PayrollModel>();

        
        public ICommand LoadEmployeesCommand { get; }
        public ICommand LoadPayrollsCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand EditEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }

        public ICommand GeneratePayrollCommand { get; }
        public ICommand OpenPayrollSettingsCommand { get; }
        public ICommand AddPayrollCommand { get; }
        public ICommand EditPayrollCommand { get; }
        public ICommand DeletePayrollCommand { get; }

        public ICommand RecalculatePayrollCommand { get; }



        // Selected items
        private EmployeeModel _selectedEmployee;
        public EmployeeModel SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
                
                ((RelayCommand)EditEmployeeCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteEmployeeCommand).RaiseCanExecuteChanged();
            }
        }

        private PayrollModel _selectedPayroll;
        public PayrollModel SelectedPayroll
        {
            get => _selectedPayroll;
            set
            {
                _selectedPayroll = value;
                OnPropertyChanged(nameof(SelectedPayroll));
                
                ((RelayCommand)EditPayrollCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeletePayrollCommand).RaiseCanExecuteChanged();
            }
        }


        private DateTime _payrollPeriod = DateTime.Today;
        public DateTime PayrollPeriod
        {
            get => _payrollPeriod;
            set { _payrollPeriod = value; OnPropertyChanged(nameof(PayrollPeriod)); }
        }

        public AdminViewModel()
        {
            
            LoadEmployeesCommand = new RelayCommand(_ => LoadEmployees());
            AddEmployeeCommand = new RelayCommand(_ => AddEmployee());
            EditEmployeeCommand = new RelayCommand(_ => EditEmployee(), _ => SelectedEmployee != null);
            DeleteEmployeeCommand = new RelayCommand(_ => DeleteEmployee(), _ => SelectedEmployee != null);

            
            GeneratePayrollCommand = new RelayCommand(_ => GeneratePayroll());
            RecalculatePayrollCommand = new RelayCommand(_ => RecalculatePayroll());
            OpenPayrollSettingsCommand = new RelayCommand(_ => OpenPayrollSettings());

           
            LoadPayrollsCommand = new RelayCommand(_ => LoadPayrolls());
            AddPayrollCommand = new RelayCommand(_ => AddPayroll());
            EditPayrollCommand = new RelayCommand(_ => EditPayroll(), _ => SelectedPayroll != null);
            DeletePayrollCommand = new RelayCommand(_ => DeletePayroll(), _ => SelectedPayroll != null);

            
            LoadEmployees();
            LoadPayrolls();
        }

        #region Employee Methods
        private void LoadEmployees()
        {
            Employees.Clear();
            foreach (var e in DatabaseHelper.GetAllEmployees())
                Employees.Add(e);
        }

        private void AddEmployee()
        {
            var dlg = new EmployeeDialog();
            if (dlg.ShowDialog() == true)
            {
                DatabaseHelper.AddEmployee(dlg.Employee);
                LoadEmployees();
            }
        }

        private void EditEmployee()
        {
            if (SelectedEmployee == null) return;
            var dlg = new EmployeeDialog(SelectedEmployee);
            if (dlg.ShowDialog() == true)
            {
                DatabaseHelper.UpdateEmployee(dlg.Employee);
                LoadEmployees();
            }
        }

        private void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            string msg = ((string)Application.Current.Resources["ConfirmDeleteMessage"])
                                 .Replace("{0}", SelectedEmployee.FullName);
            string caption = (string)Application.Current.Resources["ConfirmDeleteTitle"];

            if (MessageBox.Show(msg, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                DatabaseHelper.DeleteEmployee(SelectedEmployee.EmployeeID);
                LoadEmployees();
            }
        }
        #endregion

        #region Payroll Methods
        private void LoadPayrolls()
        {
            Payrolls.Clear();
            foreach (var p in DatabaseHelper.GetAllPayrolls())
                Payrolls.Add(p);
        }



        private void AddPayroll()
        {
            var dlg = new PayrollDialog(new PayrollModel());
            dlg.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (dlg.ShowDialog() == true)
            {
                DatabaseHelper.AddPayroll(dlg.Payroll);
                LoadPayrolls();
            }
        }

        private void EditPayroll()
        {
            if (SelectedPayroll == null) return;

            var dlg = new PayrollDialog(SelectedPayroll)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
            };
            if (dlg.ShowDialog() == true)
            {
                DatabaseHelper.UpdatePayroll(dlg.Payroll);
                LoadPayrolls();
            }
        }

         private void DeletePayroll()
        {
            if (SelectedPayroll == null) return;

            string msg = ((string)Application.Current.Resources["ConfirmDeletePayrollMessage"])
                                 .Replace("{0}", SelectedPayroll.EmployeeID.ToString());
            string caption = (string)Application.Current.Resources["ConfirmDeleteTitle"];

            if (MessageBox.Show(msg, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                DatabaseHelper.DeletePayroll(SelectedPayroll.PayrollID);
                LoadPayrolls();
            }
        }

        private void GeneratePayroll()
        {
            try
            {
                DateTime start = PayrollPeriod;        
                DateTime end = start.AddMonths(1);   

                DatabaseHelper.GeneratePayrollForPeriod(start, end);
                LoadPayrolls();

                MessageBox.Show(
                    string.Format((string)Application.Current.Resources["PayrollGeneratedMessage"], start.ToString("Y")),
                    (string)Application.Current.Resources["SuccessTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format((string)Application.Current.Resources["PayrollErrorMessage"], ex.Message),
                    (string)Application.Current.Resources["ErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RecalculatePayroll()
        {
            try
            {
                var start = PayrollPeriod.Date;
                var end = start.AddMonths(1);

           
                DatabaseHelper.DeletePayrollsInRange(start, end);

         
                DatabaseHelper.GeneratePayrollForPeriod(start, end);


                LoadPayrolls();

                MessageBox.Show(
                    string.Format((string)Application.Current.Resources["PayrollRecalculatedMessage"], start.ToString("d")),
                    (string)Application.Current.Resources["SuccessTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format((string)Application.Current.Resources["PayrollErrorMessage"], ex.Message),
                    (string)Application.Current.Resources["ErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void OpenPayrollSettings()
        {
            var dlg = new PayrollSettingsDialog
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
            };
            if (dlg.ShowDialog() == true)
                LoadPayrolls();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }
}