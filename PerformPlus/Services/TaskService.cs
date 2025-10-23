using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformPlus.Models;
using Microsoft.Data.SqlClient;

namespace PerformPlus.Services
{
    public static class TaskService
    {
        public static List<TaskAssignment> GetAssignmentsForLeader(int leaderId)
        {
            var list = new List<TaskAssignment>();
            using var conn = DatabaseHelper.GetConnection();
            const string sql = @"
      SELECT ta.AssignmentID,
             ta.TaskID,
             t.Title,
             ta.EmployeeID,
             e.FullName AS EmployeeName,
             ta.AssignedAt,
             ta.Status,
             ta.CompletedAt,
             ta.ApprovedAt
        FROM TaskAssignments ta
   INNER JOIN Tasks t     ON t.TaskID = ta.TaskID
   INNER JOIN Employees e ON e.EmployeeID = ta.EmployeeID
       WHERE ta.AssignedBy = @leaderId
    ORDER BY ta.AssignedAt DESC";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@leaderId", leaderId);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new TaskAssignment
                {
                    AssignmentID = rdr.GetInt32(0),
                    TaskID = rdr.GetInt32(1),
                    TaskTitle = rdr.GetString(2),
                    EmployeeID = rdr.GetInt32(3),
                    EmployeeName = rdr.GetString(4),
                    AssignedAt = rdr.IsDBNull(5) ? null : (DateTime?)rdr.GetDateTime(5),
                    Status = rdr.GetString(6),
                    CompletedAt = rdr.IsDBNull(7) ? null : (DateTime?)rdr.GetDateTime(7),
                    ApprovedAt = rdr.IsDBNull(8) ? null : (DateTime?)rdr.GetDateTime(8)
                });
            }
            return list;
        }


        public static List<TaskAssignment> GetAssignmentsForEmployee(int employeeId)
        {
            var list = new List<TaskAssignment>();
            using var conn = DatabaseHelper.GetConnection();
            const string sql = @"
      SELECT ta.AssignmentID,
             ta.TaskID,
             t.Title,
             ta.AssignedAt,
             ta.Status,
             ta.CompletedAt
        FROM TaskAssignments ta
   INNER JOIN Tasks t ON t.TaskID = ta.TaskID
       WHERE ta.EmployeeID = @empId
    ORDER BY t.DueDate";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@empId", employeeId);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new TaskAssignment
                {
                    AssignmentID = rdr.GetInt32(0),
                    TaskID = rdr.GetInt32(1),
                    TaskTitle = rdr.GetString(2),
                    AssignedAt = rdr.GetDateTime(3),
                    Status = rdr.GetString(4),
                    CompletedAt = rdr.IsDBNull(5) ? null : (DateTime?)rdr.GetDateTime(5)
                });
            }
            return list;
        }

        public static int CreateTaskAndGetId(string title, string desc, DateTime? due, DateTime? createdAt)
        {
            using var conn = DatabaseHelper.GetConnection();
            const string sql = @"
      INSERT INTO Tasks (Title, Description, Status, CreatedAt, DueDate)
      VALUES (@t, @d, 'Pending', @c, @due);
      SELECT CAST(SCOPE_IDENTITY() AS int);";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@t", title);
            cmd.Parameters.AddWithValue("@d", desc);
            cmd.Parameters.AddWithValue("@c", createdAt ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@due", due ?? (object)DBNull.Value);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public static void AssignTask(TaskAssignment a)
        {
            using var conn = DatabaseHelper.GetConnection();
            const string sql = @"
      INSERT INTO TaskAssignments
        (TaskID, EmployeeID, AssignedAt, AssignedBy, Status)
      VALUES
        (@task, @emp, @at, @by, @s);";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@task", a.TaskID);
            cmd.Parameters.AddWithValue("@emp", a.EmployeeID);
            cmd.Parameters.AddWithValue("@at", a.AssignedAt);
            cmd.Parameters.AddWithValue("@by", a.AssignedBy);
            cmd.Parameters.AddWithValue("@s", a.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public static void UpdateAssignment(TaskAssignment a)
        {
            using var conn = DatabaseHelper.GetConnection();
            const string sql = @"
      UPDATE TaskAssignments
         SET Status      = @s,
             CompletedAt = @comp,
             ApprovedAt  = @app
       WHERE AssignmentID = @id;";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", a.Status);
            cmd.Parameters.AddWithValue("@comp", a.CompletedAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@app", a.ApprovedAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id", a.AssignmentID);
            conn.Open();
            cmd.ExecuteNonQuery();
        }


    }



}
