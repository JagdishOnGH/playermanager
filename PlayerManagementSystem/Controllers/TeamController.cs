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
            var teams = await _context.Teams.Include(p=>p.PersonalDetails).ThenInclude(r=>r.Role).ToListAsync();
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
                var team = await _context.Teams.Include(t => t.PersonalDetails).FirstOrDefaultAsync(t => t.TeamId == teamId);
                var person = await _context.PersonalDetails.FirstOrDefaultAsync(p => p.Id == personId);
                if (team == null)
                {
                    return NotFound(new ApiResponse<string> { Error = "Team not found" });
                }

                if (person == null)
                {
                    return NotFound(new ApiResponse<string> { Error = "Person not found" });
                }

                if (team.PersonalDetails.Contains(person))
                {
                    return BadRequest(new ApiResponse<string> { Error = "Person already in team" });
                }

                if (team.PersonalDetails.Count >= 11)
                {
                    return BadRequest(new ApiResponse<string> { Error = "Team is full" });
                }

                if (person.RoleId == 1 || person.RoleId == 2)
                {
                    return BadRequest(new ApiResponse<string> { Error = "Person is manager or coach" });
                }

                team.PersonalDetails.Add(person);
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<Teams> { Data = team });
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

                var teams = await query.ToListAsync();
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