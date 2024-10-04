using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers
{
    public class TeamController : ControllerBase
    {
        private readonly EfDbContext _context;

        public TeamController(EfDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
           //i need name of the team, name of the manager, name of the coach, names of players in the team
           
              var teams = await _context.Teams.Include(t => t.PersonalDetails).ToListAsync();
              
            var toReturn = new ApiResponse<List<Teams>>();
            toReturn.Data = teams;
            return Ok(toReturn);
        }
        
        
        
        
        //post, takes team id and player id from params and adds player to team
        //making sure that person is not already in a team (find role of person ensure a team will have only one manager and coach) 
        //if person is already in a team, return suitable information message
        //if team is full, return suitable information message
        //if person is manager or coach, return suitable information message and team already have them 
        //team has PersonDetails list, add person to the list
        //post, takes team id and personId from params and removes player from team
        
        [HttpGet]
        [Route("addplayer")]
        public async Task<IActionResult> AddPlayer([FromQuery] int teamId, [FromQuery] int personId)
        {
            try
            {
                var team = await _context.Teams
                    .Include(p=>p.PersonalDetails)
                    .ThenInclude(p=>p.Role)
                    .FirstOrDefaultAsync(t => t.TeamId == teamId);
                if(team == null)
                {
                    return NotFound(new ApiResponse<string> { Error = "Team not found" });
                }
                var personData =  _context.PersonalDetails.AsQueryable();
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
                    await _context.SaveChangesAsync();
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
                await _context.SaveChangesAsync();
                
                
                
                
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
                var query = _context.Teams.AsQueryable();

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
                _context.Teams.Add(team);
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<Teams> { Data = team });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message, message = "Failed"});
            }
        }
    }
}