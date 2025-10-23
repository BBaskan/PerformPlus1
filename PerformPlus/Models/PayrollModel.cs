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

namespace PerformPlus.Models
{
    public class PayrollModel
    {
        public int PayrollID { get; set; }
        public int EmployeeID { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal SGKDeduction { get; set; }
        public decimal UnemploymentInsurance { get; set; }
        public decimal TaxBase { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal StampDuty { get; set; }
        public decimal MealAllowance { get; set; }
        public decimal TravelAllowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal PerformanceAdjustment { get; set; }
        public decimal Deductions { get; set; }
        public int OvertimeHours { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PayrollPeriodStart { get; set; }
        public DateTime PayrollPeriodEnd { get; set; }
       
    }
}