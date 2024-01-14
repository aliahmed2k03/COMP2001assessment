using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly string connectionString;
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
                    string sql = "SELECT Profiles.ProfileID,Profiles.ProfileName,Profiles.Bio,Profiles.[Location],Activities.ActivityName FROM CW2.[Users] JOIN CW2.[Profiles] ON Users.UserID = Profiles.UserID JOIN CW2.[FavouriteActivities] ON Profiles.ProfileID = FavouriteActivities.ProfileID JOIN CW2.[Activities] ON FavouriteActivities.ActivityID = Activities.ActivityID WHERE Profiles.IsArchived = 0 AND Profiles.PendingDeletion = 0 ORDER by ProfileID;";
                    using (var command = new SqlCommand(sql, connection)) { 
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) { 
                                Models.Profiles profile = new Models.Profiles();
                                profile.ProfileId = reader.GetInt32(0);
                                profile.ProfileName = reader.GetString(1);
                                profile.Bio=reader.GetString(2);
                                profile.Location=reader.GetString(3);
                                profile.FavouriteActivity=reader.GetString(4);
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
                    string sql = "SELECT Profiles.ProfileID,Profiles.ProfileName,Profiles.Bio,Profiles.[Location],\r\nActivities.ActivityName\r\nFROM CW2.[Users]\r\nJOIN CW2.[Profiles] ON Users.UserID = Profiles.UserID\r\nJOIN CW2.[FavouriteActivities] ON Profiles.ProfileID =\r\nFavouriteActivities.ProfileID\r\nJOIN CW2.[Activities] ON FavouriteActivities.ActivityID =\r\nActivities.ActivityID\r\nWHERE Profiles.IsArchived = 0 AND Profiles.PendingDeletion = 0 AND Profiles.ProfileID ="+id+" ;";
                    using (var command = new SqlCommand(sql, connection)) { 
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) { 
                                Models.Profiles profile = new Models.Profiles();
                                profile.ProfileId = reader.GetInt32(0);
                                profile.ProfileName = reader.GetString(1);
                                profile.Bio=reader.GetString(2);
                                profile.Location=reader.GetString(3);
                                profile.FavouriteActivity = reader.GetString(4);
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
            try { 
                using (var connection = new SqlConnection(connectionString)) { 
                   connection.Open();
                   string sql = "EXEC CW2.InsertProfile "+profileid+", 6, @ProfileName,@Bio,@Location, 0,0\r\nEXEC CW2.InsertFavouriteActivity @ActivityID, "+profileid+";";
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

        // PUT api/<ProfilesController>/5
        [HttpPut("{id}")]
        public IActionResult EditProfile(int id, Models.ProfileDTO profileDTO)
        {
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

        // DELETE api/<ProfilesController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteProfile(int id)
        {
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
         
    }
}
