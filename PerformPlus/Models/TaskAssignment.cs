using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformPlus.Models
{
    public class TaskAssignment
    {
        public int AssignmentID { get; set; }
        public int TaskID { get; set; }
        public string TaskTitle { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? AssignedAt { get; set; }

        public int AssignedBy { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedBy { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

    }
}
