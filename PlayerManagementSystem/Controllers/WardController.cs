using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.DTOs;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helper;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WardController(EfDbContext context) : ControllerBase
{
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllMunicipals()
    {
        try
        {
            var wards = await context.Wards.Include(w => w.Municipality).ToListAsync();
            var toReturn = new ApiResponse<List<Ward>> { Data = wards };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse(e.Message);

            return NotFound(error);
        }
    }

    
    [HttpGet]
    [Route("myteams")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetMyTeam()
    {
        try
        {
            if(!User.HasClaim("Role", TerritoryType.Ward.ToString()))
            {
                var error = SharedHelper.CreateErrorResponse("You are not authorized to perform this action");
                return BadRequest(error);
            }
            
            var validationErr = SharedHelper.ModelValidationCheck(ModelState);
            if (validationErr != null)
            {
                var error = SharedHelper.CreateErrorResponse(validationErr);
                return BadRequest(error);
            }
            var tokenWardId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (tokenWardId == null)
            {
                var error = SharedHelper.CreateErrorResponse("Ward not found");
                return BadRequest(error);
            }
            var myTeam = await context.Teams.FirstOrDefaultAsync(x => x.TerritoryId == Guid.Parse(tokenWardId));
            if (myTeam == null)
            {
                var error = SharedHelper.CreateErrorResponse("Team not found");
                return BadRequest(error);
            }
            var myPlayers = await context.PersonTeams
                .Include(pt => pt.Person)
                .Where(pt => pt.TeamId == myTeam.TeamId)
                .Select(pt => pt.Person)
                .ToListAsync();
            //categorise players
            var dataToReturn = new Dictionary<string,object>
            {
                { "players", myPlayers.Where(p => p.Role == Role.Player).ToList() },
                { "coaches", myPlayers.Where(p => p.Role == Role.Coach).ToList() },
                { "managers", myPlayers.Where(p => p.Role == Role.Manager).ToList() },
                { "teamName", myTeam.Name },
            };
            
            
            
            var toReturn = new ApiResponse<Dictionary<string,object>> { Data = dataToReturn };
            return Ok(toReturn);
            
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse("Something went wrong");
            return StatusCode(500, error);
        }
    }
}
