using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformPlus.Models;

namespace PerformPlus.Services
{
    public static class UserService
    {
        public static List<EmployeeModel> GetAll()
            => DatabaseHelper.GetAllEmployees();
    }
}