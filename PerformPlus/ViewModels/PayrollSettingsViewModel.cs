using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using PerformPlus.Models;
using PerformPlus.Services;

namespace PerformPlus.ViewModels
{
    public class PayrollSettingsViewModel : INotifyPropertyChanged
    {
        private PayrollDefaultsModel _defaults;
        public decimal MealAllowance
        {
            get => _defaults.MealAllowance;
            set { _defaults.MealAllowance = value; OnPropertyChanged(nameof(MealAllowance)); }
        }
        public decimal TravelAllowance
        {
            get => _defaults.TravelAllowance;
            set { _defaults.TravelAllowance = value; OnPropertyChanged(nameof(TravelAllowance)); }
        }
        public decimal DefaultBonus
        {
            get => _defaults.DefaultBonus;
            set { _defaults.DefaultBonus = value; OnPropertyChanged(nameof(DefaultBonus)); }
        }
        public decimal OvertimeHourlyRate
        {
            get => _defaults.OvertimeHourlyRate;
            set { _defaults.OvertimeHourlyRate = value; OnPropertyChanged(nameof(OvertimeHourlyRate)); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string n) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        public PayrollSettingsViewModel()
        {
            
            _defaults = PayrollDefaultsService.GetDefaults();
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => {  });
        }

        private void Save()
        {
            PayrollDefaultsService.SaveDefaults(_defaults);
        }
    }
}