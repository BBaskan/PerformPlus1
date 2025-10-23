using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformPlus.Models
{
    public class PayrollDefaultsModel
    {
        public int DefaultsID { get; set; }
        public decimal MealAllowance { get; set; }
        public decimal TravelAllowance { get; set; }
        public decimal DefaultBonus { get; set; }
        public decimal OvertimeHourlyRate { get; set; }
    }
}