using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
public class ProvinceController(EfDbContext context) : ControllerBase
{
    // efcontext


    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllProvince()
    {
        try
        {
            var provinces = await context.Provinces.Select(province =>
                new
                {
                    provinceName = province.Name,  // Use province.Name to get the name
                    districts = context.Districts
                        .Where(d => d.ProvinceId == province.ProvinceId)
                        .Select(d => d.Name)
                        .ToList(),
                    team= context.Teams.Where(t => t.TerritoryId == province.ProvinceId).Select(s =>
                  new  {
                        teamName = s.Name,
                        players= context.PersonTeams.Where(p=> p.TeamId==s.TeamId).Select(p=>p.Person).ToList()
                    }).FirstOrDefault()// Return the list of district names
                }).ToListAsync();

            
            var toReturn = new ApiResponse<object> { Data = provinces };
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
            if (!User.HasClaim("Role", TerritoryType.Province.ToString()))
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

            var tokenWardId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (tokenWardId == null)
            {
                var error = SharedHelper.CreateErrorResponse("Province not found");
                return BadRequest(error);
            }

            var myTeam = await context.Teams.FirstOrDefaultAsync(x =>
                x.TerritoryId == Guid.Parse(tokenWardId)
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

    // protected route, only province can access, add district under province
    [HttpPost]
    [Route("add-district")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> AddDistrict(DistrictDto district)
    {
        try
        {
            if (!User.HasClaim("Role", TerritoryType.Province.ToString()))
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

            var tokenProvinceId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (tokenProvinceId == null)
            {
                var error = SharedHelper.CreateErrorResponse("Province not found");
                return BadRequest(error);
            }

            var province = await context.Provinces.FirstOrDefaultAsync(x =>
                x.ProvinceId == Guid.Parse(tokenProvinceId)
            );
            if (province == null)
            {
                var error = SharedHelper.CreateErrorResponse("Province not found");
                return BadRequest(error);
            }

            var newDistrict = new District
            {
                Name = district.DistrictName,
                ProvinceId = Guid.Parse(tokenProvinceId),
            };
            var team = new Team
            {
                TeamId = Guid.NewGuid(),
                Name = district.DistrictName + "Team",
                TerritoryId = newDistrict.DistrictId,
                TerritoryType = TerritoryType.District,
            };
            //
            context.Districts.Add(newDistrict);
            context.Teams.Add(team);
            await context.SaveChangesAsync();
            var toReturn = new ApiResponse<District> { Data = newDistrict };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse("Something went wrong");

            return StatusCode(500, error);
        }
    }

    //protected route, only province can access, add person under district
    [HttpPost]
    [Route("add-person")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> AddPerson(Guid playerId)
    {
        try
        {
            if (!User.HasClaim("Role", TerritoryType.Municipality.ToString()))
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

            var tokenProvId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (tokenProvId == null)
            {
                var error = SharedHelper.CreateErrorResponse("Municipality not found");
                return BadRequest(error);
            }

            var provinceGuid = Guid.Parse(tokenProvId);
            var playerDetails = await context
                .Persons.Where(p => p.PersonId == playerId)
                .Select(p => new
                {
                    Player = p,
                    ProvinceTeam = context.Teams.FirstOrDefault(t => t.TerritoryId == provinceGuid),
                    ExistingPersonTeam = context.PersonTeams.FirstOrDefault(pt =>
                        pt.PersonId == playerId && pt.Team.TerritoryId == provinceGuid
                    ),
                    PlayerTeam = context
                        .PersonTeams.Include(pt => pt.Team)
                        .Where(pt =>
                            pt.PersonId == playerId && pt.Team.TerritoryType == TerritoryType.District
                        )
                        .Select(pt => new
                        {
                            Team = pt.Team,
                            District = context.Districts.FirstOrDefault(m =>
                                m.ProvinceId == pt.Team.TerritoryId
                            ),
                        })
                        .FirstOrDefault(),
                })
                .FirstOrDefaultAsync();
            if (playerDetails?.Player == null)
            {
                return BadRequest(SharedHelper.CreateErrorResponse("Player not found"));
            }

            if (playerDetails.ProvinceTeam == null)
            {
                return BadRequest(SharedHelper.CreateErrorResponse("District's team not found"));
            }

            if (playerDetails.ExistingPersonTeam != null)
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse("Player is already part of this team")
                );
            }

            if (
                playerDetails.PlayerTeam?.District!= null
                && playerDetails.PlayerTeam.District.ProvinceId != provinceGuid
            )
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse("Player is not in the correct district")
                );
            }

            // Assign the player to the district team
            var newPersonTeam = new PersonTeam
            {
                PersonId = playerId,
                TeamId = playerDetails.ProvinceTeam.TeamId,
            };

            context.PersonTeams.Add(newPersonTeam);
            await context.SaveChangesAsync();

            return Ok(new ApiResponse<Person> { Data = playerDetails.Player });
        }
        
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse("Something went wrong");
            return StatusCode(500, error);
        }
    }
}
