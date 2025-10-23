using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformPlus.Models;
using Microsoft.Data.SqlClient;
using PerformPlus.ViewModels;
using PerformPlus.Views;
using PerformPlus.Services;

namespace PerformPlus.Services
{
    public class PayrollCalculator
    {
        public PayrollModel CalculatePayroll(EmployeeModel emp, DateTime start, DateTime end)
        {
            
            var defaults = PayrollDefaultsService.GetDefaults();

            
            decimal gross = emp.BaseSalary;
            decimal sgkDeduct = gross * 0.14m;
            decimal uiDeduct = gross * 0.01m;
            decimal taxBase = gross - (sgkDeduct + uiDeduct);

            
            decimal taxAmount = CalculateProgressiveTax(taxBase);

           
            decimal stampDuty = gross * 0.00759m;

            
            decimal meal = defaults.MealAllowance;
            decimal travel = defaults.TravelAllowance;
            decimal bonus = defaults.DefaultBonus;




            List<OvertimeEntry> overtimeEntries = OvertimeService.GetApprovedForPayroll(emp.EmployeeID, end);
            decimal ovtHrs = overtimeEntries.Sum(e => e.Hours);
            decimal ovtPay = ovtHrs * defaults.OvertimeHourlyRate;



            decimal net = gross
                          + meal + travel + bonus + ovtPay
                          - (sgkDeduct + uiDeduct + taxAmount + stampDuty);

            
            return new PayrollModel
            {
                EmployeeID = emp.EmployeeID,
                GrossSalary = Math.Round(gross, 2, MidpointRounding.AwayFromZero),
                SGKDeduction = Math.Round(sgkDeduct, 2, MidpointRounding.AwayFromZero),
                UnemploymentInsurance = Math.Round(uiDeduct, 2, MidpointRounding.AwayFromZero),
                TaxBase = Math.Round(taxBase, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                StampDuty = Math.Round(stampDuty, 2, MidpointRounding.AwayFromZero),
                MealAllowance = Math.Round(meal, 2, MidpointRounding.AwayFromZero),
                TravelAllowance = Math.Round(travel, 2, MidpointRounding.AwayFromZero),
                Bonus = Math.Round(bonus, 2, MidpointRounding.AwayFromZero),
                OvertimeHours = (int)ovtHrs,
                OvertimePay = Math.Round(ovtPay, 2, MidpointRounding.AwayFromZero),
                NetSalary = Math.Round(net, 2, MidpointRounding.AwayFromZero),
                PayrollPeriodStart = start,
                PayrollPeriodEnd = end,
                PaymentDate = start
            };
        }

        private decimal CalculateProgressiveTax(decimal baseAmt)
        {
            using var conn = DatabaseHelper.GetConnection();
            const string sql = @"
                SELECT LowerLimit, UpperLimit, TaxRate
                  FROM TaxBrackets
              ORDER BY LowerLimit";
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                decimal low = rdr.GetDecimal(0);
                decimal up = rdr.GetDecimal(1);
                decimal pct = rdr.GetDecimal(2) / 100m;
                if (baseAmt >= low && baseAmt <= up)
                    return baseAmt * pct;
            }
            return 0m;
        }
    }
}
