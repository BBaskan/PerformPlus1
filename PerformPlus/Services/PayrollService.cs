using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformPlus.Models;

namespace PerformPlus.Services
{
    public class PayrollService
    {
        public void GeneratePayrolls(DateTime periodStart, DateTime periodEnd)
        {
            
            List<EmployeeModel> employees = DatabaseHelper.GetAllEmployees();
            var calculator = new PayrollCalculator();

            foreach (var employee in employees)
            {
               
                PayrollModel payroll = calculator.CalculatePayroll(employee, periodStart, periodEnd);

                
                DatabaseHelper.AddPayroll(payroll);
            }
        }
    }
}

