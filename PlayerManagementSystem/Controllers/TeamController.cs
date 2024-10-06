using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers
{
    public class TeamController(EfDbContext context) : ControllerBase
    {
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
           //i need name of the team, name of the manager, name of the coach, names of players in the team
           
              var teams = await context.Teams.Include(t => t.PersonalDetails).ToListAsync();
              
            var toReturn = new ApiResponse<List<Teams>>();
            toReturn.Data = teams;
            return Ok(toReturn);
        }
        
        
        
        
        
        [HttpGet]
        [Route("addplayer")]
        public async Task<IActionResult> AddPlayer([FromQuery] int teamId, [FromQuery] int personId)
        {
            try
            {
                var team = await context.Teams
                    .Include(p=>p.PersonalDetails)
                    .ThenInclude(p=>p.Role)
                    .FirstOrDefaultAsync(t => t.TeamId == teamId);
                if(team == null)
                {
                    return NotFound(new ApiResponse<string> { Error = "Team not found" });
                }
                var personData =  context.PersonalDetails.AsQueryable();
                var person = await personData
                    .Include(p=>p.Role)
                    .FirstOrDefaultAsync(p => p.Id == personId);
                if(person == null)
                {
                    return NotFound(new ApiResponse<string> { Error = "Person not found" });
                }

                if (team.PersonalDetails.Count > 14)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Data = "Team is full"
                    });
                }
                //check if person is already in a team
                if (team.PersonalDetails.Any(p => p.Id == person.Id))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Data = "Person is already in a team"
                    });
                }
                //add person to team if role is player
                if (person.Role.RoleName.Equals("Player"))
                {
                    team.PersonalDetails.Add(person);
                    await context.SaveChangesAsync();
                    return Ok(new ApiResponse<string> { Data = "Player added to team" });
                }
                
                //check if team already has a manager or coach
                if (team.PersonalDetails.Any(p => p.Role.RoleName.Equals(person.Role.RoleName)))
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Error = "Team already has a " + person.Role.RoleName
                    });

                }
                //add person to team if role is manager or coach
                team.PersonalDetails.Add(person);
                await context.SaveChangesAsync();
                
                
                
                
                return Ok(new ApiResponse<string> { Data = $"{person.Role.RoleName} added to team" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message });
            }
        }
        

        [HttpGet]
        [Route("filter")]
        public async Task<IActionResult> GetAll([FromQuery] int? teamId)
        {
            try
            {
                var query = context.Teams.AsQueryable();

                if (teamId.HasValue)
                {
                    query = query.Where(p => p.TeamId == teamId.Value);
                }

                var teams = await query
                    .Include(p=>p.PersonalDetails)
                    .ToListAsync();
                var toReturn = new ApiResponse<List<Teams>>
                {
                    Data = teams
                };
                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Teams team)
        {
            try
            {
                context.Teams.Add(team);
                await context.SaveChangesAsync();
                return Ok(new ApiResponse<Teams> { Data = team });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message, message = "Failed"});
            }
        }
    }
}