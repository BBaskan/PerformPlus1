using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformPlus.Services;
using PerformPlus.Models;
using PerformPlus.ViewModels;
using PerformPlus.Views;
using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace PerformPlus.Models
{
    public class EmployeeModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _fullName;
        private string _username;
        private string _passwordHash;
        private string _role;
        private int? _teamID;
        private DateTime _hireDate;
        private int _points;
        private string _email;
        private string _maritalStatus;
        private int _numberOfChildren;

        public int EmployeeID { get; set; }
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }
        public string PasswordHash
        {
            get => _passwordHash;
            set { _passwordHash = value; OnPropertyChanged(nameof(PasswordHash)); }
        }
        public string Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(nameof(Role)); }
        }
        public int? TeamID
        {
            get => _teamID;
            set { _teamID = value; OnPropertyChanged(nameof(TeamID)); }
        }
        public DateTime HireDate
        {
            get => _hireDate;
            set { _hireDate = value; OnPropertyChanged(nameof(HireDate)); }
        }
        public int Points
        {
            get => _points;
            set { _points = value; OnPropertyChanged(nameof(Points)); }
        }
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string MaritalStatus
        {
            get => _maritalStatus;
            set { _maritalStatus = value; OnPropertyChanged(nameof(MaritalStatus)); }
        }

        public int NumberOfChildren
        {
            get => _numberOfChildren;
            set { _numberOfChildren = value; OnPropertyChanged(nameof(NumberOfChildren)); }
        }

        private decimal _baseSalary;
        public decimal BaseSalary
        {
            get => _baseSalary;
            set { _baseSalary = value; OnPropertyChanged(nameof(BaseSalary)); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string errorMessage = null;
                switch (columnName)
                {
                    case nameof(Email):
                       
                        var emailErrorMsg = (string)Application.Current.Resources["InvalidEmailMessage"];
                        if (string.IsNullOrWhiteSpace(Email) ||
                            !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        {
                            errorMessage = emailErrorMsg;
                        }
                        break;
                    case nameof(PasswordHash):
                        
                        var passwordErrorMsg = (string)Application.Current.Resources["InvalidPasswordMessage"];
                        if (string.IsNullOrWhiteSpace(PasswordHash) ||
                            !Regex.IsMatch(PasswordHash, @"^(?=.*[A-Z])(?=.*\d).+$"))
                        {
                            errorMessage = passwordErrorMsg;
                        }
                        break;
                       
                }
                return errorMessage;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}