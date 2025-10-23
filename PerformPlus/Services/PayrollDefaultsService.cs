using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using PerformPlus.Models;

namespace PerformPlus.Services
{
    public static class PayrollDefaultsService
    {
        private const string ConnStr = ""; //"Use a json file or put your database string here"

        public static PayrollDefaultsModel GetDefaults()
        {
            const string sql = @"
                SELECT TOP 1 
                    DefaultsID, 
                    MealAllowance, 
                    TravelAllowance, 
                    DefaultBonus, 
                    OvertimeHourlyRate 
                FROM PayrollDefaults
                ORDER BY DefaultsID DESC";

            using var conn = new SqlConnection(ConnStr);
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();

            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new PayrollDefaultsModel
                {
                    DefaultsID = rdr.GetInt32(rdr.GetOrdinal("DefaultsID")),
                    MealAllowance = rdr.GetDecimal(rdr.GetOrdinal("MealAllowance")),
                    TravelAllowance = rdr.GetDecimal(rdr.GetOrdinal("TravelAllowance")),
                    DefaultBonus = rdr.GetDecimal(rdr.GetOrdinal("DefaultBonus")),
                    OvertimeHourlyRate = rdr.GetDecimal(rdr.GetOrdinal("OvertimeHourlyRate"))
                };
            }

           
            return new PayrollDefaultsModel();
        }

        public static void SaveDefaults(PayrollDefaultsModel m)
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            if (m.DefaultsID > 0)
            {
                const string updateSql = @"
                    UPDATE PayrollDefaults
                    SET 
                        MealAllowance       = @meal,
                        TravelAllowance     = @travel,
                        DefaultBonus        = @bonus,
                        OvertimeHourlyRate  = @overtime
                    WHERE DefaultsID = @id";

                using var cmd = new SqlCommand(updateSql, conn);
                cmd.Parameters.AddWithValue("@meal", m.MealAllowance);
                cmd.Parameters.AddWithValue("@travel", m.TravelAllowance);
                cmd.Parameters.AddWithValue("@bonus", m.DefaultBonus);
                cmd.Parameters.AddWithValue("@overtime", m.OvertimeHourlyRate);
                cmd.Parameters.AddWithValue("@id", m.DefaultsID);
                cmd.ExecuteNonQuery();
            }
            else
            {
                const string insertSql = @"
                    INSERT INTO PayrollDefaults
                        (MealAllowance, TravelAllowance, DefaultBonus, OvertimeHourlyRate)
                    VALUES
                        (@meal, @travel, @bonus, @overtime)";

                using var cmd = new SqlCommand(insertSql, conn);
                cmd.Parameters.AddWithValue("@meal", m.MealAllowance);
                cmd.Parameters.AddWithValue("@travel", m.TravelAllowance);
                cmd.Parameters.AddWithValue("@bonus", m.DefaultBonus);
                cmd.Parameters.AddWithValue("@overtime", m.OvertimeHourlyRate);
                cmd.ExecuteNonQuery();
            }
        }
    }
}