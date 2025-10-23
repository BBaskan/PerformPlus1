using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformPlus.Models;

namespace PerformPlus.Services
{
    public static class OvertimeService
    {
        internal static List<OvertimeEntry> GetRecentEntries(int leaderId)
        {
            return new List<OvertimeEntry>();
        }
        public static void AddEntry(OvertimeEntry e) {  }
        public static void ApproveEntry(int entryId, int leaderId) {  }

        internal static List<OvertimeEntry> GetApprovedForPayroll(int employeeID, DateTime end)
        {
            
            return new List<OvertimeEntry>();
           
        }
    }
}
