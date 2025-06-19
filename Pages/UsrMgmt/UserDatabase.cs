using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ASDS_dev.Pages.UsrMgmt
{
    public class User
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserRole { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string Pass{ get; set; }
    }

    internal class UserDatabase
    {
        private static readonly string dbPath =@"D:\ASDS_dev\UserDatabase.db";

        // Initialize DB
        public static void InitializeDatabase()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId TEXT PRIMARY KEY,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    UserRole TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    FailedAttempts INTEGER DEFAULT 0
                );
                ";
                tableCommand.ExecuteNonQuery();
            }
        }

        // Add user
        public static void AddUser(string userId, string firstName, string lastName, string role, string status,string pass)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText =
                @"
                INSERT INTO Users (UserId, FirstName, LastName, UserRole, Status, Password, CreatedAt)
                VALUES ($id, $first, $last, $role, $status, $pass, $created);
                ";

                insertCommand.Parameters.AddWithValue("$id", userId);
                insertCommand.Parameters.AddWithValue("$first", firstName);
                insertCommand.Parameters.AddWithValue("$last", lastName);
                insertCommand.Parameters.AddWithValue("$role", role);
                insertCommand.Parameters.AddWithValue("$status", status);
                insertCommand.Parameters.AddWithValue("$pass", pass);
                insertCommand.Parameters.AddWithValue("$created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                insertCommand.ExecuteNonQuery();
            }
        }

        // Get all users
        public static ObservableCollection<User> GetAllUsers()
        {
            var users = new ObservableCollection<User>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM Users";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = reader["UserId"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            UserRole = reader["UserRole"].ToString(),
                            Status = reader["Status"].ToString(),
                            CreatedAt = reader["CreatedAt"].ToString()
                        });
                    }
                }
            }

            return users;
        }




public static void DeleteUser(string userId)
    {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();

            var command = new SqliteCommand("DELETE FROM Users WHERE UserId = @UserId", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.ExecuteNonQuery();
        }
    }


        public static void UpdateUser(string userId, string firstName, string lastName, string role, string status)
{
    using (var connection = new SqliteConnection($"Data Source={dbPath}"))
    {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Users
            SET FirstName = @FirstName,
                LastName = @LastName,
                UserRole = @UserRole,
                Status = @Status
            WHERE UserId = @UserId";

        command.Parameters.AddWithValue("@FirstName", firstName);
        command.Parameters.AddWithValue("@LastName", lastName);
        command.Parameters.AddWithValue("@UserRole", role);
        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@UserId", userId);

        command.ExecuteNonQuery();
    }
}
        public static bool RegisterUser(string userId, string firstName, string lastName, string password, string role = "User", string status = "Active")
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE UserId = @UserId";
                checkCommand.Parameters.AddWithValue("@UserId", userId);
                var count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count > 0)
                    return false; // User already exists

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText =
                @"
        INSERT INTO Users (UserId, FirstName, LastName, UserRole, Status, Password, CreatedAt)
        VALUES (@UserId, @FirstName, @LastName, @UserRole, @Status, @Password, @CreatedAt);
        ";
                insertCommand.Parameters.AddWithValue("@UserId", userId);
                insertCommand.Parameters.AddWithValue("@FirstName", firstName);
                insertCommand.Parameters.AddWithValue("@LastName", lastName);
                insertCommand.Parameters.AddWithValue("@UserRole", role);
                insertCommand.Parameters.AddWithValue("@Status", status);
                insertCommand.Parameters.AddWithValue("@Password", password);
                insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                insertCommand.ExecuteNonQuery();
                return true;
            }
        }


        public class LoginResult
        {
            public bool IsSuccess { get; set; }
            public bool IsUserNotFound { get; set; }
            public bool IsPasswordIncorrect { get; set; }
            public bool IsSuspended { get; set; }
            public string UserRole { get; set; }
            public string Status { get; set; }
        }

        public static LoginResult ValidateLogin(string userId, string password)
        {
            var result = new LoginResult();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Password, UserRole, Status ,FailedAttempts FROM Users WHERE UserId = @UserId";
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        result.IsUserNotFound = true;
                        return result;
                    }

                    string storedPassword = reader["Password"].ToString();
                    result.UserRole = reader["UserRole"].ToString();
                    result.Status = reader["Status"].ToString();
                    int failedAttempts = Convert.ToInt32(reader["FailedAttempts"]);

                    if (string.Equals(result.Status, "Suspended", StringComparison.OrdinalIgnoreCase))
                    {
                        result.IsSuspended = true;
                        return result;
                    }
                    // You can add hashing here instead of plain comparison in production
                    if (!PasswordHelper.VerifyPassword(password, storedPassword))
                    {
                        failedAttempts++;

                        // Update the failed attempts count
                        var updateCmd = connection.CreateCommand();
                        updateCmd.CommandText = @"
                    UPDATE Users 
                    SET FailedAttempts = @FailedAttempts, 
                        Status = CASE WHEN @FailedAttempts >= 3 THEN 'Suspended' ELSE Status END 
                    WHERE UserId = @UserId";
                        updateCmd.Parameters.AddWithValue("@FailedAttempts", failedAttempts);
                        updateCmd.Parameters.AddWithValue("@UserId", userId);
                        updateCmd.ExecuteNonQuery();

                        if (failedAttempts >= 3)
                        {
                            result.IsSuspended = true;
                        }
                        else
                        {
                            result.IsPasswordIncorrect = true;
                        }

                        return result;
                    }

                    var resetCmd = connection.CreateCommand();
                    resetCmd.CommandText = "UPDATE Users SET FailedAttempts = 0 WHERE UserId = @UserId";
                    resetCmd.Parameters.AddWithValue("@UserId", userId);
                    resetCmd.ExecuteNonQuery();
                    result.IsSuccess = true;
                    return result;
                }
            }
        }



        public static bool ValidateUser(string userId, string inputPassword)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = @"
            SELECT Password FROM Users
            WHERE UserId = $id;
        ";
                selectCommand.Parameters.AddWithValue("$id", userId);

                var result = selectCommand.ExecuteScalar();
                if (result != null)
                {
                    string storedHashedPassword = result.ToString();
                    return PasswordHelper.VerifyPassword(inputPassword, storedHashedPassword);
                }

                return false;
            }
        }


        // Update user password
        public static bool UpdateUserPassword(string userId, string newPassword)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = @"
            UPDATE Users
            SET Password = $hashed
            WHERE UserId = $id;
        ";

                string hashedPassword = PasswordHelper.HashPassword(newPassword);

                updateCommand.Parameters.AddWithValue("$hashed", hashedPassword);
                updateCommand.Parameters.AddWithValue("$id", userId);

                int affectedRows = updateCommand.ExecuteNonQuery();
                return affectedRows > 0;
            }
        }








    }
}
