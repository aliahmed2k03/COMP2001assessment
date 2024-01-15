using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
        private readonly IConfiguration _configuration;
        
        public UsersController(IConfiguration configuration) { 
            connectionString = configuration["ConnectionStrings:SqlServerDb"]?? "";
            _configuration = configuration;

        }
        
        
        // POST api/<UsersController>
        [HttpPost("Register")]
        public IActionResult registerUser(Models.UsersDTO usersdto)
        {
            CreateSalt(out string passwordSalt);
            string passwordHash =CreatePasswordHash(usersdto.Password,passwordSalt);
            string userId = getUserId();
            try { 
                using (var connection = new SqlConnection(connectionString)) {
                    connection.Open();
                    string sql = "EXEC CW2.InsertUser "+userId+",@Username,@Email,@UserPassword,0,@passwordSalt;";
                    using(var command = new SqlCommand(sql, connection)) { 
                        command.Parameters.AddWithValue("@Username",usersdto.Username);
                        command.Parameters.AddWithValue("@Email",usersdto.Email);
                        command.Parameters.AddWithValue("@UserPassword",passwordHash);
                        command.Parameters.AddWithValue("@passwordSalt",passwordSalt);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex){ 
                ModelState.AddModelError("Users","Sorry, this is not available right now");
                return BadRequest(ModelState);
            }
            Users.Id = userId;
            Users.Username = usersdto.Username;
            Users.Email = usersdto.Email;
            Users.PasswordHash=passwordHash;
            Users.PasswordSalt=passwordSalt;
            Users.isAdmin = 0;
            Users.authenticated = false;
            return Ok("username:"+Users.Username+"\nEmail: "+Users.Email+"\nPasswordHash: "+Users.PasswordHash+"\nPasswordSalt: "+Users.PasswordSalt+"\nIsAdmin:"+Users.isAdmin);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> loginUser(Models.UsersDTO usersdto)
        {
            string passwordSalt = null;
            string authenticated = null;
            try { 
                using (var connection = new SqlConnection(connectionString)) { 
                    connection.Open();
                    string sql  = "SELECT * FROM CW2.Users;";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        
                        foreach(DataRow row in dataTable.Rows)
                        {                                            
                            
                            if (row["Username"].ToString() == usersdto.Username && row["Email"].ToString() == usersdto.Email) {
                                passwordSalt = row["PasswordSalt"].ToString();
                                
                                if (row["UserPassword"].ToString() == CreatePasswordHash(usersdto.Password, passwordSalt)) { 
                                    Users.Id = row["UserID"].ToString();
                                    Users.Username = usersdto.Username;   
                                    Users.Email = usersdto.Email;
                                    Users.PasswordHash= CreatePasswordHash(usersdto.Password, passwordSalt);
                                    Users.PasswordSalt=passwordSalt;
                                    Users.isAdmin = Convert.ToInt32(row["IsAdmin"]);
                                    Users.authenticated = true;
                                    authenticated = await Authenticator(usersdto.Email,usersdto.Password);
                                    return Ok("You have logged in succesfully, UserID: "+Users.Id+"\nAuthentication API Response: "+ authenticated);
                                          
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
            return Ok("Your username,email or password is incorrect");
           
        }
            
        private string CreatePasswordHash(string password, string passwordSalt ) { 
            using (var hmac = new HMACSHA512(System.Text.Encoding.UTF8.GetBytes(passwordSalt))) { 
                byte[] hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }
        
        private void CreateSalt(out string passwordSalt) { 
            using (var hmac = new HMACSHA512())
            {
                byte[] salt = hmac.Key;
                passwordSalt = Convert.ToBase64String(salt);
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

        private string CreateJWTtoken(Models.Users user) { 
            List<Claim> claims = new List<Claim> { 
                new Claim("Id",Users.Id),
                new Claim("Username",Users.Username),
                new Claim("Email", Users.Email)
                };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }
        public static async Task<string> Authenticator(string email, string password) { 
            string url = "https://web.socem.plymouth.ac.uk/COMP2001/auth/api/users";
            var requestdata = new {Email = email, Password = password};
            string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(requestdata);
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(jsondata,Encoding.UTF8, "application/json");
            using var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode) { 
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else
                return "Authentication failed";
        }  
        
    }
}
