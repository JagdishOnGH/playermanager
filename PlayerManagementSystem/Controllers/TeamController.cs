using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helper;
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
            var error = SharedHelper.CreateErrorResponse(e.Message);
            return NotFound(error);
        }
    }

    [HttpPost]
    [Route("assign/{teamId:guid}/")]
    public async Task<IActionResult> AssignPlayerToTeam([FromRoute] Guid teamId, Guid playerId)
    {
        try
        {
            var toBeAssignedTeam = await context.Teams.FirstOrDefaultAsync(x => x.TeamId == teamId);
            if (toBeAssignedTeam == null)
                return BadRequest(SharedHelper.CreateErrorResponse("Team does not exist"));

            if (toBeAssignedTeam.TerritoryType == TerritoryType.Ward)
                return BadRequest(
                    SharedHelper.CreateErrorResponse("Player can't be assigned to wards")
                );

            var person = await GetPlayerCurrentTeam(playerId, toBeAssignedTeam.TerritoryType);
            if (person == null)
                return BadRequest(
                    SharedHelper.CreateErrorResponse(
                        "Person does not exist or is not in the correct team"
                    )
                );

            var isValidAssignment = await ValidateTerritoryAssignment(
                person.Team.TerritoryId,
                toBeAssignedTeam
            );
            if (!isValidAssignment)
                return BadRequest(SharedHelper.CreateErrorResponse("Player can't be assigned"));
            await SharedHelper.ValidateTeamRolesAsync(teamId, person.Person.Role, context);

            await AssignPlayer(person.PersonId, toBeAssignedTeam.TeamId);
            return Ok(new ApiResponse<string> { Data = "Player assigned to team" });
        }
        catch (Exception e)
        {
            return NotFound(SharedHelper.CreateErrorResponse(e.Message));
        }
    }

    private async Task<PersonTeam?> GetPlayerCurrentTeam(
        Guid playerId,
        TerritoryType teamTerritoryType
    )
    {
        var requiredType = teamTerritoryType switch
        {
            TerritoryType.Province => TerritoryType.District,
            TerritoryType.Municipality => TerritoryType.Ward,
            TerritoryType.District => TerritoryType.Municipality,
            TerritoryType.Ward => 
             throw new ArgumentOutOfRangeException(),
        };

        return await context
            .PersonTeams.Include(t => t.Team)
            .Include(p => p.Person)
            .FirstOrDefaultAsync(x =>
                x.PersonId == playerId && x.Team.TerritoryType == requiredType
            );
    }

    private async Task<bool> ValidateTerritoryAssignment(
        Guid currentTerritoryId,
        Team toBeAssignedTeam
    )
    {
        return toBeAssignedTeam.TerritoryType switch
        {
            TerritoryType.Province => await GetProvinceId(currentTerritoryId)
                == toBeAssignedTeam.TerritoryId,
            TerritoryType.Municipality => await GetMunicipleId(currentTerritoryId)
                == toBeAssignedTeam.TerritoryId,
            TerritoryType.District => await GetDistrictId(currentTerritoryId)
                == toBeAssignedTeam.TerritoryId,
            _ => false,
        };
    }

    private async Task AssignPlayer(Guid playerId, Guid teamId)
    {
        var newPerson = new PersonTeam { PersonId = playerId, TeamId = teamId };
        context.PersonTeams.Add(newPerson);
        await context.SaveChangesAsync();
    }

    async Task<Guid> GetProvinceId(Guid districtId)
    {
        var province = await context.Districts.FirstOrDefaultAsync(x => x.DistrictId == districtId);
        if (province == null)
        {
            throw new Exception("District does not exist");
        }
        return province.ProvinceId;
    }

    async Task<Guid> GetDistrictId(Guid muncipleId)
    {
        var district = await context.Municipalities.FirstOrDefaultAsync(x =>
            x.MunicipalityId == muncipleId
        );
        if (district == null)
        {
            throw new Exception("Municipality does not exist");
        }
        return district.DistrictId;
    }

    async Task<Guid> GetMunicipleId(Guid wardId)
    {
        var municiple = await context.Wards.FirstOrDefaultAsync(x => x.WardId == wardId);

        if (municiple == null)
        {
            throw new Exception("Ward does not exist");
        }
        return municiple.MunicipalityId;
    } //
}
