using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    [Route("addwards")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> AddWard(WardDto ward)
    {
        try
        {
            if (!User.HasClaim("Role", "Municipality"))
            {
                var error = SharedHelper.CreateErrorResponse(
                    "You are not authorized to perform this action"
                );
                return BadRequest(error);
            }
            var tokenMunicipalId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (tokenMunicipalId == null)
            {
                var error = SharedHelper.CreateErrorResponse("Municipality not found");
                return BadRequest(error);
            }

            var validationErr = SharedHelper.ModelValidationCheck(ModelState);
            if (validationErr != null)
            {
                var error = SharedHelper.CreateErrorResponse(validationErr);
                return BadRequest(error);
            }
            //use muncipal id and check if same word no exists or not
            var wardQuery = context
                .Wards.Where(m => m.MunicipalityId == Guid.Parse(tokenMunicipalId))
                .AsQueryable();
            var wardNoExist = await wardQuery.FirstOrDefaultAsync(x => x.WardNo == ward.WardNo);

            if (wardNoExist != null)
            {
                var error = SharedHelper.CreateErrorResponse(
                    $"Ward No  already exists with {ward.WardNo}"
                );

                return BadRequest(error);
            }
            var myWard = new Ward
            {
                WardId = Guid.NewGuid(),
                MunicipalityId = Guid.Parse(tokenMunicipalId),
                WardNo = ward.WardNo,
            };
            var municipality = await context.Municipalities.FirstOrDefaultAsync(x =>
                x.MunicipalityId == Guid.Parse(tokenMunicipalId)
            );
            if (municipality == null)
            {
                var error = SharedHelper.CreateErrorResponse(
                    $"Municipality does not exist with id {tokenMunicipalId} "
                );
                return NotFound(error);
            }

            context.Teams.Add(
                new Team
                {
                    TeamId = Guid.NewGuid(),
                    Name = $"{municipality.Name} Ward No {ward.WardNo}'s Team",
                    TerritoryId = myWard.WardId,
                    TerritoryType = TerritoryType.Ward,
                }
            );
            await context.Wards.AddAsync(myWard);
            await context.SaveChangesAsync();
            return Ok(myWard);
        }
        catch (Exception e)
        {
            var error = new ApiResponse<string> { Error = e.Message };
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
