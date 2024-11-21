using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helper;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistrictController(EfDbContext context) : ControllerBase
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllDistricts()
    {
        try
        {
            var districts = await context.Districts.Include(d => d.Province).ToListAsync();
            var toReturn = new ApiResponse<List<District>> { Data = districts };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = new ApiResponse<string> { Error = e.Message + "HEllo" };
            return BadRequest(error);
        }
    }

    [HttpPost]
    [Route("add-municipality")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> AddDistrict(string municipalityName)
    {
        try
        {
            if (!User.HasClaim("Role", "District"))
            {
                var error = SharedHelper.CreateErrorResponse(
                    "You are not authorized to perform this action"
                );
                return BadRequest(error);
            }

            var validationErr = SharedHelper.ModelValidationCheck(ModelState);
            if (validationErr != null)
            {
                var error = SharedHelper.CreateErrorResponse(validationErr);
                return BadRequest(error);
            }
            var tokenDistrictId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (tokenDistrictId == null)
            {
                var error = SharedHelper.CreateErrorResponse("District not found");
                return BadRequest(error);
            }

            var municipality = new Municipality
            {
                MunicipalityId = Guid.NewGuid(),
                DistrictId = Guid.Parse(tokenDistrictId),
                Name = municipalityName.ToUpper(),
            };
            var team = new Team
            {
                TeamId = Guid.NewGuid(),
                Name = municipalityName + " Team",
                TerritoryId = municipality.MunicipalityId,
                TerritoryType = TerritoryType.Municipality,
            };

            context.Municipalities.Add(municipality);
            context.Teams.Add(team);
            await context.SaveChangesAsync();
            var toReturn = new ApiResponse<Municipality> { Data = municipality };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse("Something went wrong");

            return StatusCode(500, error);
        }
    }

    //myplayers
    [HttpGet]
    [Route("myteams")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetMyTeam()
    {
        try
        {
            var tokenDistrictId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (tokenDistrictId == null)
            {
                var error = SharedHelper.CreateErrorResponse("District not found");
                return BadRequest(error);
            }
            var myTeam = await context.Teams.FirstOrDefaultAsync(x =>
                x.TerritoryId == Guid.Parse(tokenDistrictId)
            );
            if (myTeam == null)
            {
                var error = SharedHelper.CreateErrorResponse("Team not found");
                return BadRequest(error);
            }
            var myPlayers = await context
                .PersonTeams.Include(pt => pt.Person)
                .Where(pt => pt.TeamId == myTeam.TeamId)
                .Select(pt => pt.Person)
                .ToListAsync();
            //categorise players
            var dataToReturn = new Dictionary<string, object>
            {
                { "players", myPlayers.Where(p => p.Role == Role.Player).ToList() },
                { "coaches", myPlayers.Where(p => p.Role == Role.Coach).ToList() },
                { "managers", myPlayers.Where(p => p.Role == Role.Manager).ToList() },
                { "teamName", myTeam.Name },
            };

            var toReturn = new ApiResponse<Dictionary<string, object>> { Data = dataToReturn };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse("Something went wrong");
            return StatusCode(500, error);
        }
    }
}
