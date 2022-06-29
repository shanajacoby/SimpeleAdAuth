
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Data
{
    public class AdsRepository
    {
        private string _connectionString;
        public AdsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                "VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(password));
            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if(user== null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;

        }
        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE Email=@email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new User
            {
                UserId= (int)reader["UserId"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"]
            };

        }
        public void NewAd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ad VALUES (@userId, @phoneNumber, @text, @date)";
            cmd.Parameters.AddWithValue("@userId", ad.UserId);
          
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@text", ad.Text);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void DeleteAd(int adId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Ad WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", adId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Ad> GetAllAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name FROM Ad a JOIN Users u on a.UserId =u.UserId";
            connection.Open();
            var ads = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id=(int)reader["Id"],
                    UserId=(int)reader["UserId"],
                    UserName=(string)reader["Name"],
                    DateSubmitted=(DateTime)reader["DateSubmitted"],
                    PhoneNumber=(string)reader["PhoneNumber"],
                    Text=(string)reader["Text"]
                });
            }
            return ads;
        }
        public List<Ad> GetAdsForId(int? userId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name FROM Ad a JOIN Users u on a.UserId =u.UserId WHERE a.UserId=@id";
            cmd.Parameters.AddWithValue("@id", userId);
            connection.Open();
            var ads = new List<Ad>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    UserId = (int)reader["UserId"],
                    UserName = (string)reader["Name"],
                    DateSubmitted = (DateTime)reader["DateSubmitted"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Text = (string)reader["Text"]
                });
            }
            return ads;
        }
    }
}
