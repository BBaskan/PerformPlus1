using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PerformPlus.Services;
using PerformPlus.Models;
using PerformPlus.ViewModels;
using PerformPlus.Views;
using System.Windows;




namespace PerformPlus.Services  
{

    public static class SessionManager
    {
        public static int EmployeeID { get; set; }
        public static string FullName { get; set; }
        public static string Role { get; set; }
    }


    public class DatabaseHelper
    {
        private static readonly string connectionString = ""; //"Use a json file or put your database string here"

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static List<EmployeeModel> GetAllEmployees()
        {
            var list = new List<EmployeeModel>();
            using var conn = GetConnection();
            string query = @"
        SELECT EmployeeID, FullName, Username, PasswordHash, Role, TeamID, Points, Email, HireDate, MaritalStatus, NumberOfChildren, BaseSalary
        FROM Employees";
            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new EmployeeModel
                {
                    EmployeeID = rdr.GetInt32(0),
                    FullName = rdr.GetString(1),
                    Username = rdr.GetString(2),
                    PasswordHash = rdr.GetString(3),
                    Role = rdr.GetString(4),
                    TeamID = rdr.IsDBNull(5) ? null : (int?)rdr.GetInt32(5),
                    Points = rdr.GetInt32(6),
                    Email = rdr.GetString(7),
                    HireDate = rdr.GetDateTime(8),
                    MaritalStatus = rdr.GetString(9),
                    NumberOfChildren = rdr.GetInt32(10),
                    BaseSalary = rdr.GetDecimal(11)
                });
            }
            return list;
        }

       
        public static List<PayrollModel> GetAllPayrolls()
        {
            var list = new List<PayrollModel>();
            using var conn = GetConnection();
            string query =
     "SELECT PayrollID, EmployeeID, GrossSalary, SGKDeduction, UnemploymentInsurance, " +
     "TaxBase, TaxAmount, StampDuty, MealAllowance, TravelAllowance, Bonus, PerformanceAdjustment, " +
     "Deductions, OvertimeHours, OvertimePay, NetSalary, PaymentDate, PayrollPeriodStart, PayrollPeriodEnd " +
     "FROM Payroll";

            using var cmd = new SqlCommand(query, conn);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new PayrollModel 
                {
                    PayrollID = rdr.GetInt32(0),
                    EmployeeID = rdr.GetInt32(1),
                    GrossSalary = rdr.GetDecimal(2),
                    SGKDeduction = rdr.GetDecimal(3),
                    UnemploymentInsurance = rdr.GetDecimal(4),
                    TaxBase = rdr.GetDecimal(5),
                    TaxAmount = rdr.GetDecimal(6),
                    StampDuty = rdr.GetDecimal(7),
                    MealAllowance = rdr.GetDecimal(8),
                    TravelAllowance = rdr.GetDecimal(9),
                    Bonus = rdr.GetDecimal(10),
                    PerformanceAdjustment = rdr.GetDecimal(11),
                    Deductions = rdr.GetDecimal(12),
                    OvertimeHours = rdr.GetInt32(13),
                    OvertimePay = rdr.GetDecimal(14),
                    NetSalary = rdr.GetDecimal(15),
                    PaymentDate = rdr.GetDateTime(16),
                    PayrollPeriodStart = rdr.GetDateTime(17),
                    PayrollPeriodEnd = rdr.GetDateTime(18)
                });
            }
            return list;
        }

        public static void GeneratePayrollForPeriod(DateTime start, DateTime end)
        {
            var employees = GetAllEmployees();       
            var calc = new PayrollCalculator();

            foreach (var emp in employees)
            {
               
                using var checkConn = new SqlConnection(connectionString);
                using var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Payroll " +
                    "WHERE EmployeeID=@id AND PayrollPeriodStart=@s AND PayrollPeriodEnd=@e",
                    checkConn);
                checkCmd.Parameters.AddWithValue("@id", emp.EmployeeID);
                checkCmd.Parameters.AddWithValue("@s", start);
                checkCmd.Parameters.AddWithValue("@e", end);

                checkConn.Open();
                var exists = (int)checkCmd.ExecuteScalar();
                if (exists > 0) continue;

                
                var payroll = calc.CalculatePayroll(emp, start, end);

                
                AddPayroll(payroll);
            }
        }




        private static decimal CalculateTax(decimal taxBase, SqlConnection conn)
        {
            var cmd = new SqlCommand("SELECT * FROM TaxBrackets ORDER BY LowerLimit", conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    decimal lower = (decimal)reader["LowerLimit"];
                    decimal upper = (decimal)reader["UpperLimit"];
                    decimal rate = (decimal)reader["TaxRate"];

                    if (taxBase >= lower && taxBase <= upper)
                    {
                        return taxBase * (rate / 100m);
                    }
                }
            }
            return 0m;
        }

        public static void AddEmployee(EmployeeModel employee)
        {
            try
            {
                using var conn = GetConnection();
                string query = @"
        INSERT INTO Employees 
            (FullName, Username, PasswordHash, Role, TeamID, Points, Email, HireDate, MaritalStatus, NumberOfChildren, BaseSalary)
        VALUES 
            (@FullName, @Username, @PasswordHash, @Role, @TeamID, @Points, @Email, @HireDate, @MaritalStatus, @NumberOfChildren, @BaseSalary)";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", employee.FullName);
                cmd.Parameters.AddWithValue("@Username", employee.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", employee.PasswordHash);
                cmd.Parameters.AddWithValue("@Role", employee.Role);
                cmd.Parameters.AddWithValue("@TeamID", (object)employee.TeamID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Points", employee.Points);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
                cmd.Parameters.AddWithValue("@MaritalStatus", employee.MaritalStatus);
                cmd.Parameters.AddWithValue("@NumberOfChildren", employee.NumberOfChildren);
                cmd.Parameters.AddWithValue("@BaseSalary", employee.BaseSalary); 

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in AddEmployee: {ex.Message}");
                throw;
            }
        }

        public static void UpdateEmployee(EmployeeModel employee)
        {
            using var conn = GetConnection();
            string query = @"
    UPDATE Employees 
    SET FullName = @FullName, 
        Username = @Username, 
        PasswordHash = @PasswordHash, 
        Role = @Role, 
        TeamID = @TeamID, 
        Points = @Points, 
        Email = @Email,
        HireDate = @HireDate,
        MaritalStatus = @MaritalStatus,
        NumberOfChildren = @NumberOfChildren,
        BaseSalary = @BaseSalary -- ✅ added
    WHERE EmployeeID = @EmployeeID";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FullName", employee.FullName);
            cmd.Parameters.AddWithValue("@Username", employee.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", employee.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", employee.Role);
            cmd.Parameters.AddWithValue("@TeamID", (object)employee.TeamID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Points", employee.Points);
            cmd.Parameters.AddWithValue("@Email", employee.Email);
            cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
            cmd.Parameters.AddWithValue("@MaritalStatus", employee.MaritalStatus);
            cmd.Parameters.AddWithValue("@NumberOfChildren", employee.NumberOfChildren);
            cmd.Parameters.AddWithValue("@BaseSalary", employee.BaseSalary); 
            cmd.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        
        public static void DeleteEmployee(int employeeID)
        {
            using var conn = GetConnection();
            string query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public static void AddPayroll(PayrollModel payroll)
        {
            using var conn = GetConnection();
            string query = @"
        INSERT INTO Payroll 
            (EmployeeID, GrossSalary, SGKDeduction, UnemploymentInsurance, TaxBase, TaxAmount, StampDuty, 
             MealAllowance, TravelAllowance, Bonus, PerformanceAdjustment, Deductions, OvertimeHours, 
             OvertimePay, NetSalary, PaymentDate, PayrollPeriodStart, PayrollPeriodEnd)
        VALUES 
            (@EmployeeID, @GrossSalary, @SGKDeduction, @UnemploymentInsurance, @TaxBase, @TaxAmount, @StampDuty, 
             @MealAllowance, @TravelAllowance, @Bonus, @PerformanceAdjustment, @Deductions, @OvertimeHours, 
             @OvertimePay, @NetSalary, @PaymentDate, @PayrollPeriodStart, @PayrollPeriodEnd)";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@EmployeeID", payroll.EmployeeID);
            cmd.Parameters.AddWithValue("@GrossSalary", payroll.GrossSalary);
            cmd.Parameters.AddWithValue("@SGKDeduction", payroll.SGKDeduction);
            cmd.Parameters.AddWithValue("@UnemploymentInsurance", payroll.UnemploymentInsurance);
            cmd.Parameters.AddWithValue("@TaxBase", payroll.TaxBase);
            cmd.Parameters.AddWithValue("@TaxAmount", payroll.TaxAmount);
            cmd.Parameters.AddWithValue("@StampDuty", payroll.StampDuty);
            cmd.Parameters.AddWithValue("@MealAllowance", payroll.MealAllowance);
            cmd.Parameters.AddWithValue("@TravelAllowance", payroll.TravelAllowance);
            cmd.Parameters.AddWithValue("@Bonus", payroll.Bonus);
            cmd.Parameters.AddWithValue("@PerformanceAdjustment", payroll.PerformanceAdjustment);
            cmd.Parameters.AddWithValue("@Deductions", payroll.Deductions);
            cmd.Parameters.AddWithValue("@OvertimeHours", payroll.OvertimeHours);
            cmd.Parameters.AddWithValue("@OvertimePay", payroll.OvertimePay);
            cmd.Parameters.AddWithValue("@NetSalary", payroll.NetSalary);
            cmd.Parameters.AddWithValue("@PaymentDate", payroll.PaymentDate);
            cmd.Parameters.AddWithValue("@PayrollPeriodStart", payroll.PayrollPeriodStart);
            cmd.Parameters.AddWithValue("@PayrollPeriodEnd", payroll.PayrollPeriodEnd);



            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public static void UpdatePayroll(PayrollModel p)
        {
            using var conn = GetConnection();
            const string sql = @"
      UPDATE Payroll
      SET 
        SGKDeduction        = @sgk,
        UnemploymentInsurance=@ui,
        TaxBase             = @taxBase,
        TaxAmount           = @taxAmount,
        StampDuty           = @stamp,
        MealAllowance       = @meal,
        TravelAllowance     = @travel,
        Bonus               = @bonus,
        PerformanceAdjustment=@perf,
        Deductions          = @deductions,
        OvertimeHours       = @ovtHrs,
        OvertimePay         = @ovtPay,
        NetSalary           = @net,
        PaymentDate         = @payDate
      WHERE PayrollID = @id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sgk", p.SGKDeduction);
            cmd.Parameters.AddWithValue("@ui", p.UnemploymentInsurance);
            cmd.Parameters.AddWithValue("@taxBase", p.TaxBase);
            cmd.Parameters.AddWithValue("@taxAmount", p.TaxAmount);
            cmd.Parameters.AddWithValue("@stamp", p.StampDuty);
            cmd.Parameters.AddWithValue("@meal", p.MealAllowance);
            cmd.Parameters.AddWithValue("@travel", p.TravelAllowance);
            cmd.Parameters.AddWithValue("@bonus", p.Bonus);
            cmd.Parameters.AddWithValue("@perf", p.PerformanceAdjustment);
            cmd.Parameters.AddWithValue("@deductions", p.Deductions);
            cmd.Parameters.AddWithValue("@ovtHrs", p.OvertimeHours);
            cmd.Parameters.AddWithValue("@ovtPay", p.OvertimePay);
            cmd.Parameters.AddWithValue("@net", p.NetSalary);
            cmd.Parameters.AddWithValue("@payDate", p.PaymentDate);
            cmd.Parameters.AddWithValue("@id", p.PayrollID);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public static void DeletePayroll(int payrollID)
        {
            using var conn = GetConnection();
            const string sql = @"
        DELETE FROM Payroll
        WHERE PayrollID = @PayrollID";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@PayrollID", payrollID);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public static void DeletePayrollsInRange(DateTime startDate, DateTime endDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DELETE FROM Payroll WHERE PayrollPeriodStart >= @StartDate AND PayrollPeriodEnd <= @EndDate", connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.ExecuteNonQuery();
                }
            }
        }



    }
}