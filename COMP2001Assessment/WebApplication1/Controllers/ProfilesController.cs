using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly string connectionString;
        public Users user = new Users();
        public ProfilesController(IConfiguration configuration) { 
            connectionString = configuration["ConnectionStrings:SqlServerDb"]?? "";
        }
        // GET: api/<ProfilesController>
        [HttpGet]
        public IActionResult getProfiles()
        {
            List<Models.Profiles> profiles = new List<Models.Profiles>();
            try { 
                using (var connection = new SqlConnection(connectionString)) { 
                    connection.Open();
                    string sql = "SELECT Profiles.ProfileID,Profiles.UserID,Profiles.ProfileName,Profiles.Bio,Profiles.[Location],Activities.ActivityName FROM CW2.[Users] JOIN CW2.[Profiles] ON Users.UserID = Profiles.UserID JOIN CW2.[FavouriteActivities] ON Profiles.ProfileID = FavouriteActivities.ProfileID JOIN CW2.[Activities] ON FavouriteActivities.ActivityID = Activities.ActivityID WHERE Profiles.IsArchived = 0 ORDER by ProfileID;";
                    using (var command = new SqlCommand(sql, connection)) { 
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) { 
                                Models.Profiles profile = new Models.Profiles();
                                profile.ProfileId = reader.GetInt32(0);
                                profile.UserId = reader.GetInt32(1);
                                profile.ProfileName = reader.GetString(2);
                                profile.Bio=reader.GetString(3);
                                profile.Location=reader.GetString(4);
                                profile.FavouriteActivity=reader.GetString(5);
                                profiles.Add(profile);
                            }            
                        }
                    }
                }
            }
            catch (Exception ex){ 
                ModelState.AddModelError("Profiles","Sorry, this is not available right now");
                return BadRequest(ModelState);
            }
            return Ok(profiles);
        }

       

        // GET api/<ProfilesController>/5
        [HttpGet("{id}")]
        public IActionResult GetProfile(int id)
        {
           List<Models.Profiles> profiles = new List<Models.Profiles>();
            try { 
                using (var connection = new SqlConnection(connectionString)) { 
                    connection.Open();
                    string sql = "SELECT Profiles.ProfileID,Profiles.UserID,Profiles.ProfileName,Profiles.Bio,Profiles.[Location],\r\nActivities.ActivityName\r\nFROM CW2.[Users]\r\nJOIN CW2.[Profiles] ON Users.UserID = Profiles.UserID\r\nJOIN CW2.[FavouriteActivities] ON Profiles.ProfileID =\r\nFavouriteActivities.ProfileID\r\nJOIN CW2.[Activities] ON FavouriteActivities.ActivityID =\r\nActivities.ActivityID\r\nWHERE Profiles.IsArchived = 0 AND Profiles.PendingDeletion = 0 AND Profiles.ProfileID ="+id+" ;";
                    using (var command = new SqlCommand(sql, connection)) { 
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) { 
                                Models.Profiles profile = new Models.Profiles();
                                profile.ProfileId = reader.GetInt32(0);
                                profile.UserId = reader.GetInt32(1);
                                profile.ProfileName = reader.GetString(2);
                                profile.Bio=reader.GetString(3);
                                profile.Location=reader.GetString(4);
                                profile.FavouriteActivity=reader.GetString(5);
                                profiles.Add(profile);
                            }            
                        }
                    }
                }
            }
            catch (Exception ex){ 
                ModelState.AddModelError("Profiles","Sorry, this is not available right now");
                return BadRequest(ModelState);
            }
            return Ok(profiles);
        }

        // POST api/<ProfilesController>
        [HttpPost]
        public IActionResult AddProfile(Models.ProfileDTO profileDTO)
        {
            string profileid = getProfileId();
            if (Users.authenticated) { 
                try { 
                    using (var connection = new SqlConnection(connectionString)) { 
                       connection.Open();
                       string sql = "EXEC CW2.InsertProfile "+profileid+","+Users.Id+", @ProfileName,@Bio,@Location, 0,0\r\nEXEC CW2.InsertFavouriteActivity @ActivityID, "+profileid+";";
                        using(var command = new SqlCommand(sql, connection)) { 
                            command.Parameters.AddWithValue("@ProfileName",profileDTO.ProfileName);
                            command.Parameters.AddWithValue("@Bio",profileDTO.Bio);
                            command.Parameters.AddWithValue("@Location",profileDTO.Location);
                            command.Parameters.AddWithValue("@ActivityID",profileDTO.FavouriteActivity);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex){ 
                    ModelState.AddModelError("Profiles","Sorry, this is not available right now");
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            else
                return BadRequest("You are not authenticated. Log in to your account");
        }

        // PUT api/<ProfilesController>/5
        [HttpPut("{id}")]
        public IActionResult EditProfile(int id, Models.ProfileDTO profileDTO)
        {
            bool validID = false;
            DataTable dt = getProfilesTable();
            if (Users.authenticated) { 
                foreach(DataRow row in dt.Rows) {
                    if (Convert.ToInt32(row["ProfileID"]) == id && Convert.ToInt32(row["UserID"]) == Convert.ToInt32(Users.Id)) { 
                        validID = true;
                        break;
                    }
                    
                }
                if (validID) { 
                    try { 
                        using (var connection = new SqlConnection(connectionString)) { 
                            connection.Open();
                            string sql = "EXEC CW2.UpdateProfile "+id+",@ProfileName, @Bio, @Location  \r\n EXEC CW2.UpdateFavouriteActivity @ActivityID, "+id+";";
                            using(var command = new SqlCommand(sql, connection)) { 
                                command.Parameters.AddWithValue("@ProfileName",profileDTO.ProfileName);
                                command.Parameters.AddWithValue("@Bio",profileDTO.Bio);
                                command.Parameters.AddWithValue("@Location",profileDTO.Location);
                                command.Parameters.AddWithValue("@ActivityID",profileDTO.FavouriteActivity);
                                command.ExecuteNonQuery();
                            
                            }
                        }
                    }
                    catch (Exception ex){ 
                        ModelState.AddModelError("Profiles","Sorry, this is not available right now");
                        return BadRequest(ModelState);
                    }
                    return Ok();
                }
                else
                    return BadRequest("This profile does not belong to you");
            }
            else
                return BadRequest("You are not authenticated. Log in to your account");
        }

        // DELETE api/<ProfilesController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteProfile(int id)
        {
            if (Users.authenticated) { 
                
                try { 
                    using (var connection = new SqlConnection(connectionString)) { 
                       connection.Open();
                       string sql = "EXEC CW2.DeleteProfile "+id+";";
                        using(var command = new SqlCommand(sql, connection)) { 
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex){ 
                    ModelState.AddModelError("Profiles","Sorry, this is not available right now");
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            else
                return BadRequest("You are not authenticated. Log in to your account");
        }
        [HttpPost("ArchiveAllProfiles")]
        public IActionResult ArchiveProfiles()
        {
            try {
                if (Users.isAdmin) { 
                    using (var connection = new SqlConnection(connectionString)) { 
                        connection.Open();
                        string sql = "EXEC CW2.ArchiveProfiles;";
                        using(var command = new SqlCommand(sql, connection)) { 
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                    return Ok("You are not an admin");
            }
            catch (Exception ex){ 
                ModelState.AddModelError("Profiles","Sorry, this is not available right now");
                return BadRequest(ModelState);
            }
            return Ok();
        }

        protected string getProfileId() { 
            int count = 0;
            using (var connection = new SqlConnection(connectionString)) { 
                connection.Open();
                string sql = "SELECT Profiles.ProfileID,Profiles.ProfileName,Profiles.Bio,Profiles.[Location],Activities.ActivityName FROM CW2.[Users] JOIN CW2.[Profiles] ON Users.UserID = Profiles.UserID JOIN CW2.[FavouriteActivities] ON Profiles.ProfileID = FavouriteActivities.ProfileID JOIN CW2.[Activities] ON FavouriteActivities.ActivityID = Activities.ActivityID ORDER by ProfileID;";
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
         
        protected DataTable getProfilesTable() { 
            using (var connection = new SqlConnection(connectionString)) 
            { 
                connection.Open();
                string sql  = "SELECT * FROM CW2.Profiles;";
                using (var command = new SqlCommand(sql, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                } 
            }
                        
        }
    }
}
