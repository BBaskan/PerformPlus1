using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformPlus.Models
{
    public class OvertimeEntry
    {
        public int EntryID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public int LeaderID { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal Hours { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedBy { get; set; }
    }

    
}
