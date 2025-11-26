using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace RailwayKiosk
{
    public class User
    {
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "User"; // "Admin" or "User"
    }

    public static class UserService
    {
        private static readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        private static Dictionary<string, User> _users = new();

        static UserService()
        {
            LoadUsers();
            
            // Ensure default admin exists and has correct privileges
            EnsureAdminUser();
        }

        private static void EnsureAdminUser()
        {
            var adminKey = "admin";
            
            // Check if admin exists (case-insensitive due to dictionary)
            if (!_users.ContainsKey(adminKey))
            {
                // Create default admin
                var hash = HashPassword("admin123");
                _users[adminKey] = new User { PasswordHash = hash, Role = "Admin" };
                SaveUsers();
            }
            else
            {
                // Force Admin role for 'admin' user
                if (_users.TryGetValue(adminKey, out var user))
                {
                    if (user.Role != "Admin")
                    {
                        user.Role = "Admin";
                        SaveUsers();
                    }
                }
            }
        }

        private static void LoadUsers()
        {
            _users = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);

            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    Dictionary<string, User>? loadedUsers = null;

                    // Try to deserialize as new format
                    try 
                    {
                        loadedUsers = JsonSerializer.Deserialize<Dictionary<string, User>>(json);
                    }
                    catch
                    {
                        // Fallback migration: If it fails, it might be the old format Dictionary<string, string>
                        var oldUsers = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                        if (oldUsers != null)
                        {
                            loadedUsers = new Dictionary<string, User>();
                            foreach (var kvp in oldUsers)
                            {
                                loadedUsers[kvp.Key] = new User { PasswordHash = kvp.Value, Role = "User" };
                            }
                            // Force save to update format
                            SaveUsers(); 
                        }
                    }

                    if (loadedUsers != null)
                    {
                        // Transfer to case-insensitive dictionary
                        foreach (var kvp in loadedUsers)
                        {
                            _users[kvp.Key] = kvp.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Failed to load users");
                }
            }
        }

        private static void SaveUsers()
        {
            try
            {
                var json = JsonSerializer.Serialize(_users);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Failed to save users");
            }
        }

        public static bool ValidateUser(string username, string password)
        {
            if (_users.TryGetValue(username, out var user))
            {
                // Extra safety: If this is the admin user, ensure they have Admin role
                if (username.Equals("admin", StringComparison.OrdinalIgnoreCase) && user.Role != "Admin")
                {
                    user.Role = "Admin";
                    SaveUsers();
                }

                return VerifyPassword(password, user.PasswordHash);
            }
            return false;
        }

        public static bool IsAdmin(string username)
        {
            if (_users.TryGetValue(username, out var user))
            {
                return user.Role == "Admin";
            }
            return false;
        }

        public static bool RegisterUser(string username, string password)
        {
            if (_users.ContainsKey(username))
            {
                return false; // User already exists
            }

            var hash = HashPassword(password);
            _users[username] = new User { PasswordHash = hash, Role = "User" };
            SaveUsers();
            return true;
        }

        // Admin Management Methods
        public static Dictionary<string, string> GetAllUsers()
        {
            // Return Dictionary of Username -> Role
            var result = new Dictionary<string, string>();
            foreach(var kvp in _users)
            {
                result[kvp.Key] = kvp.Value.Role;
            }
            return result;
        }

        public static bool DeleteUser(string username)
        {
            if (username.ToLower() == "admin") return false; // Prevent deleting main admin
            if (_users.ContainsKey(username))
            {
                _users.Remove(username);
                SaveUsers();
                return true;
            }
            return false;
        }

        public static bool UpdateUserRole(string username, string newRole)
        {
            if (username.ToLower() == "admin") return false; // Prevent modifying main admin role
            if (_users.TryGetValue(username, out var user))
            {
                user.Role = newRole;
                SaveUsers();
                return true;
            }
            return false;
        }

        private static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(storedHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
