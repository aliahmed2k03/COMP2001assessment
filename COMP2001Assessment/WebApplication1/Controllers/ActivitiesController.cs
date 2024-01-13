using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly string connectionString;
        public ActivitiesController(IConfiguration configuration) { 
            connectionString = configuration["ConnectionStrings:SqlServerDb"]?? "";
        }
        // GET: api/<ActivitiesController>
        [HttpGet]
        public IActionResult GetActivities()
        {
            List<Models.Activities> activities = new List<Models.Activities>();
            try { 
                using (var connection = new SqlConnection(connectionString)) { 
                    connection.Open();
                    string sql = "SELECT * FROM CW2.Activities;";
                    using (var command = new SqlCommand(sql, connection)) { 
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) { 
                                Models.Activities activity = new Models.Activities();
                                activity.Id = reader.GetInt32(0);
                                activity.Name = reader.GetString(1);
                                
                             
                                activities.Add(activity);
                            }            
                        }
                    }
                }
            }
            catch (Exception ex){ 
                ModelState.AddModelError("Profiles","Sorry, this is not available right now");
                return BadRequest(ModelState);
            }
            return Ok(activities);
        }

       
    }
}
