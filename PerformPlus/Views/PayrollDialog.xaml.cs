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
using PerformPlus.Models;
using PerformPlus.Services;

namespace PerformPlus.Views
{
    public partial class PayrollDialog : Window
    {
        public PayrollModel Payroll { get; }

        public PayrollDialog(PayrollModel payroll)
        {
            InitializeComponent();
            Payroll = payroll;
            DataContext = Payroll;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Recompute dependent fields:
            var defaults = PayrollDefaultsService.GetDefaults();
            Payroll.OvertimePay = Payroll.OvertimeHours * defaults.OvertimeHourlyRate;

            // Recalculate NetSalary:
            Payroll.NetSalary = Payroll.GrossSalary
                              + Payroll.MealAllowance
                              + Payroll.TravelAllowance
                              + Payroll.Bonus
                              + Payroll.OvertimePay
                              - (Payroll.SGKDeduction
                                 + Payroll.UnemploymentInsurance
                                 + Payroll.TaxAmount
                                 + Payroll.StampDuty);

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}