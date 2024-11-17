using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TeamController(EfDbContext context) : ControllerBase
{
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllTeams()
    {
        try
        {
            var teams = await context.Teams.ToListAsync();
            var toReturn = new ApiResponse<List<Team>> { Data = teams };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = new ApiResponse<string> { Error = e.Message };
            return NotFound(error);
        }
    }
    [HttpPost]
    [Route("assign/{teamId}/")]
    public async Task<IActionResult> AssignPlayerToTeam([FromQuery]Guid teamId, Guid playerId)
    {
        try
        {
            
            var team = await context.Teams.FirstOrDefaultAsync(x => x.TeamId == teamId);
            if (team == null)
            {
                var error = new ApiResponse<string> { Error = "Team does not exist" };
                return BadRequest(error);
            }
            var player = await context.Persons.FirstOrDefaultAsync(x => x.PersonId == playerId);
            if (player == null)
            {
                var error = new ApiResponse<string> { Error = "Player does not exist" };
                return BadRequest(error);
            }
            var teamPlayer = new PersonTeam { PersonId = playerId, TeamId = teamId };
            await context.PersonTeams.AddAsync(teamPlayer);
            await context.SaveChangesAsync();
            var toReturn = new ApiResponse<PersonTeam> { Data = teamPlayer };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = new ApiResponse<string> { Error = e.Message };
            return NotFound(error);
        }
    }
 
    //
}