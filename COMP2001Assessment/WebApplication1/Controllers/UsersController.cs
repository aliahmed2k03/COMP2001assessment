using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using WebApplication1.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly string connectionString;
        public UsersController(IConfiguration configuration) { 
            connectionString = configuration["ConnectionStrings:SqlServerDb"]?? "";
        }
        public static Models.Users user = new Models.Users();

        // POST api/<UsersController>
        [HttpPost("Register")]
        public IActionResult registerUser(Models.UsersDTO usersdto)
        {
            CreatePasswordHash(usersdto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            try { 
                using (var connection = new SqlConnection(connectionString)) {
                    connection.Open();
                    string sql = "EXEC CW2.InsertUser "+getUserId()+",@Username,@Email,@UserPassword,0;";
                    using(var command = new SqlCommand(sql, connection)) { 
                        command.Parameters.AddWithValue("@Username",usersdto.Username);
                        command.Parameters.AddWithValue("@Email",usersdto.Email);
                        command.Parameters.AddWithValue("@UserPassword",passwordHash);


                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex){ 
                ModelState.AddModelError("Users","Sorry, this is not available right now");
                return BadRequest(ModelState);
            }
            user.Username = usersdto.Username;
            user.Email = usersdto.Email;
            user.PasswordHash=passwordHash;
            return Ok(user);

        }

        [HttpPost("Login")]
        public IActionResult loginUser(Models.UsersDTO usersdto)
        {
            try { 
                using (var connection = new SqlConnection(connectionString)) { 
                    connection.Open();
                    string sql = "SELECT Username, Email, UserPassword FROM CW2.Users WHERE Username = @Username AND Email = @Email;";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        
                        command.Parameters.AddWithValue("@Username",usersdto.Username);
                        command.Parameters.AddWithValue("@Email",usersdto.Email);
                        using (var reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                
                                string dbUsername = reader["Username"].ToString();
                                
                                string dbPassword = reader["UserPassword"].ToString();
                                
                                string dbEmail = reader["Email"].ToString();
                                return Ok(dbUsername);
                                if (dbUsername == usersdto.Username && dbEmail == usersdto.Email)
                                {
                                    return Ok("You have logged in succesfully");
                                }
                            }
                        }
                    }    
            
                }
            } 
            catch { 
                ModelState.AddModelError("Users","Sorry, this is not available right now");
                return BadRequest(ModelState);
            }
            return Ok("Your username, Email or password is incorrect");
           
        }
            
    

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordsalt) { 
            using (var hmac = new HMACSHA512()) { 
                passwordsalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPassword(string password, byte[] dbpassword) {
            
            using (var hmac = new HMACSHA512()) { 
                var hashedpass = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return hashedpass.SequenceEqual(dbpassword);
            }
        }
        protected string getUserId() { 
            int count = 0;
            using (var connection = new SqlConnection(connectionString)) { 
                connection.Open();
                string sql = "SELECT * FROM CW2.Users;";
                using (var command = new SqlCommand(sql, connection)) { 
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) { 
                            count++;      
                        }            
                    }
                }
            }
            count = count+1;
            return count.ToString();
        }
    }
}
