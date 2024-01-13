using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
                    string sql = "SELECT Profiles.ProfileID,Profiles.ProfileName,Profiles.Bio,Profiles.[Location],Activities.ActivityName FROM CW1.[Users] JOIN CW1.[Profiles] ON Users.UserID = Profiles.UserID JOIN CW1.[FavouriteActivities] ON Profiles.ProfileID = FavouriteActivities.ProfileID JOIN CW1.[Activities] ON FavouriteActivities.ActivityID = Activities.ActivityID WHERE Profiles.IsArchived = 0 ORDER by ProfileID;";
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
                    string sql = "SELECT Profiles.ProfileID,Profiles.ProfileName,Profiles.Bio,Profiles.[Location],\r\nActivities.ActivityName\r\nFROM CW1.[Users]\r\nJOIN CW1.[Profiles] ON Users.UserID = Profiles.UserID\r\nJOIN CW1.[FavouriteActivities] ON Profiles.ProfileID =\r\nFavouriteActivities.ProfileID\r\nJOIN CW1.[Activities] ON FavouriteActivities.ActivityID =\r\nActivities.ActivityID\r\nWHERE Profiles.IsArchived = 0 AND Profiles.ProfileID ="+id+";";
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
            List<Models.Profiles> profiles = new List<Models.Profiles>();
            try { 
                using (var connection = new SqlConnection(connectionString)) { 
                    connection.Open();
                   
                }
            }
            catch (Exception ex){ 
                ModelState.AddModelError("Profiles","Sorry, this is not available right now");
                return BadRequest(ModelState);
            }
            return Ok(profiles);
        }

        // PUT api/<ProfilesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProfilesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
