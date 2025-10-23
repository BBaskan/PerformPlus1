using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformPlus.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public int TaskID { get; set; }
        public int EmployeeID { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentedAt { get; set; }

        public string Text { get; set; }           
        public DateTime CreatedAt { get; set; }    

        public string EmployeeName { get; set; } 
    }
}