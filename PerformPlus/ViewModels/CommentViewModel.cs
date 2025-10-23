using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformPlus.ViewModels
{
    public class CommentViewModel
    {
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}