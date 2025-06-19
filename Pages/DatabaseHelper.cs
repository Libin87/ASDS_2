using ASDS_dev.Pages.UsrMgmt;
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace ASDS_dev.Pages;

public static partial class DatabaseHelper
{
    //public enum LoginResult
    //{
    //    Success,
    //    UserNotFound,
    //    IncorrectPassword
    //    Status
    //}

    public class LoginResult
    {
        public bool IsSuccess { get; set; } = false;
        public bool IsUserNotFound { get; set; } = false;
        public bool IsPasswordIncorrect { get; set; } = false;
        public string Status { get; set; } = string.Empty;
        public bool IsSuspended { get; set; } = false;
    }

    private static string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ASDS.db");

    public static void InitializeDatabase()
    {
        using (SqliteConnection db = new SqliteConnection($"Data Source={dbPath}"))
        {
            db.Open();
            string tableCommand = @"CREATE TABLE IF NOT EXISTS Users2 (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        FirstName TEXT NOT NULL,
                                        LastName TEXT NOT NULL,
                                        Username TEXT UNIQUE NOT NULL,
                                        Password TEXT NOT NULL

                                    );";


            SqliteCommand createTable = new SqliteCommand(tableCommand, db);
            createTable.ExecuteNonQuery();
        }
    }

    public static bool RegisterUser(string firstName, string lastName, string username, string password)
    {
        using (SqliteConnection db = new SqliteConnection($"Data Source={dbPath}"))
        {
            db.Open();

            string checkQuery = "SELECT COUNT(*) FROM Users2 WHERE Username = @Username";
            SqliteCommand checkCmd = new SqliteCommand(checkQuery, db);
            checkCmd.Parameters.AddWithValue("@Username", username);
            var count = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (count > 0)
                return false;

            string insertQuery = "INSERT INTO Users2 (FirstName, LastName, Username, Password) VALUES (@FirstName, @LastName, @Username, @Password)";
            SqliteCommand insertCmd = new SqliteCommand(insertQuery, db);
            insertCmd.Parameters.AddWithValue("@FirstName", firstName);
            insertCmd.Parameters.AddWithValue("@LastName", lastName);
            insertCmd.Parameters.AddWithValue("@Username", username);
            insertCmd.Parameters.AddWithValue("@Password", password);
            insertCmd.ExecuteNonQuery();
        }

        return true;
    }

    public static LoginResult ValidateLogin(string username, string password)
    {
        var result = new LoginResult();

        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();

            // Check login table first
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Password FROM Users2 WHERE Username = @Username";
            command.Parameters.AddWithValue("@Username", username);

            string storedPassword = null;
            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    result.IsUserNotFound = true;
                    return result;
                }

                storedPassword = reader["Password"].ToString();
            }

            if (storedPassword != password)
            {
                result.IsPasswordIncorrect = true;
                return result;
            }

            // Check status from Users table (user profile)
            var statusCommand = connection.CreateCommand();
            statusCommand.CommandText = "SELECT Status FROM Users WHERE UserId = @UserId";
            statusCommand.Parameters.AddWithValue("@UserId", username);

            using (var reader = statusCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    string status = reader["Status"].ToString();
                    result.Status = status;

                    if (status.Equals("Suspended", StringComparison.OrdinalIgnoreCase))
                    {
                        result.IsSuspended = true;
                        return result;
                    }
                }
            }

            result.IsSuccess = true;
            return result;
        }
    }







}



