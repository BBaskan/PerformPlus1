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
    public static class CommentService
    {
        public static List<Comment> GetCommentsForTask(int taskId)
        {
            var list = new List<Comment>();
            using var conn = DatabaseHelper.GetConnection();
            const string sql = @"
                SELECT c.CommentID, c.TaskID, c.EmployeeID, c.CommentText, c.CommentedAt,
                       e.FullName AS EmployeeName
                  FROM Comments c
             LEFT JOIN Employees e ON e.EmployeeID = c.EmployeeID
                 WHERE c.TaskID = @taskId
              ORDER BY c.CommentedAt ASC";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@taskId", taskId);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Comment
                {
                    CommentID = rdr.GetInt32(0),
                    TaskID = rdr.GetInt32(1),
                    EmployeeID = rdr.GetInt32(2),
                    CommentText = rdr.GetString(3),
                    CommentedAt = rdr.GetDateTime(4),
                    EmployeeName = rdr.GetString(5)
                });
            }
            return list;
        }

        public static void AddComment(Comment c)
        {
            using var conn = DatabaseHelper.GetConnection();
            const string sql = @"
                INSERT INTO Comments (TaskID, EmployeeID, CommentText, CommentedAt)
                VALUES (@taskId, @empId, @text, @at)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@taskId", c.TaskID);
            cmd.Parameters.AddWithValue("@empId", c.EmployeeID);
            cmd.Parameters.AddWithValue("@text", c.CommentText);
            cmd.Parameters.AddWithValue("@at", c.CommentedAt);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}