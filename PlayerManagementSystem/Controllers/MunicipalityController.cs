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
public class MunicipalityController(EfDbContext context) : ControllerBase
{
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllMunicipals()
    {
        try
        {
            var districts = await context.Municipalities.Include(d => d.District).ToListAsync();
            var toReturn = new ApiResponse<List<Municipality>> { Data = districts };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse(e.Message);
            return NotFound(error);
        }
    }

    [HttpGet("available-to-add")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetAvailablePlayers()
    {
        try
        {
            // Verify user role
            if (!User.HasClaim("Role", TerritoryType.Municipality.ToString()))
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse(
                        "You are not a Municipality admin to perform this action"
                    )
                );
            }

            // Check for model validation errors
            var validationErr = SharedHelper.ModelValidationCheck(ModelState);
            if (validationErr != null)
            {
                return BadRequest(SharedHelper.CreateErrorResponse(validationErr));
            }

            // Extract district ID from the token
            var tokenMunicipality = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (string.IsNullOrEmpty(tokenMunicipality))
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse("Municipality not found in token")
                );
            }

            var municipalId = Guid.Parse(tokenMunicipality);
            var municipalityPlayers = await context
                .Wards.Where(m => m.MunicipalityId == municipalId) // Filter municipalities by district
                .Select(m2 => new
                {
                    m2.WardNo, // Replace with the actual column/property for municipality name
                    Players = context
                        .Teams.Where(t => t.TerritoryId == m2.WardId) // Filter teams by municipality
                        .SelectMany(t =>
                            context
                                .PersonTeams.Include(pt => pt.Person)
                                .Include(pt => pt.Team)
                                .Where(pt => pt.TeamId == t.TeamId)
                                .Select(pt => new
                                {
                                    name = pt.Person.FirstName + " " + pt.Person.LastName,
                                    role = pt.Person.Role.ToString(),
                                    team = pt.Team.Name,
                                    teamId = pt.Team.TeamId,
                                })
                        ) // Select Player IDs
                        .ToList() // Convert players to a list
                    ,
                })
                .ToListAsync(); // Execute query
            //            Execute query//
            // nvert the result to a list of municipalities

            var result = new ApiResponse<object> { Data = municipalityPlayers }; // Execute and materialize the result
            return Ok(result);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse("Something went wrong: " + e.Message);
            return StatusCode(500, error);
        }

        return Ok("OK");
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
            //check if wardno is number
            if (!int.TryParse(ward.WardNo, out _))
            {
                var error = SharedHelper.CreateErrorResponse("Ward No must be a number");
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
            if(!User.HasClaim("Role", TerritoryType.Municipality.ToString()))
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
                var error = SharedHelper.CreateErrorResponse("Municipality not found");
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

    [HttpPost("add-person")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    //by guuid same as dist controller
    public async Task<IActionResult> AddPerson([FromBody] Guid playerId)
    {
        try
        {
            if (!User.HasClaim("Role", TerritoryType.Municipality.ToString()))
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

            var tokenMunId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (tokenMunId == null)
            {
                var error = SharedHelper.CreateErrorResponse("Municipality not found");
                return BadRequest(error);
            }
            var munId = Guid.Parse(tokenMunId);
            var playerDetails = await context
                .Persons.Where(p => p.PersonId == playerId)
                .Select(p => new
                {
                    Player = p,
                    MunicipalityTeam = context.Teams.FirstOrDefault(t => t.TerritoryId == munId),
                    ExistingPersonTeam = context.PersonTeams.FirstOrDefault(pt =>
                        pt.PersonId == playerId && pt.Team.TerritoryId == munId
                    ),
                    PlayerTeam = context
                        .PersonTeams.Include(pt=>pt.Team).Where(pt =>
                            pt.PersonId == playerId
                            && pt.Team.TerritoryType == TerritoryType.Ward
                        )
                        .Select(pt => new
                        {
                            Team = pt.Team,
                            Ward = context.Wards.FirstOrDefault(m =>
                                m.MunicipalityId == pt.Team.TerritoryId
                            ),
                        })
                        .FirstOrDefault(),
                })
                .FirstOrDefaultAsync();
            // Validate retrieved data
            if (playerDetails?.Player == null)
            {
                return BadRequest(SharedHelper.CreateErrorResponse("Player not found"));
            }

            if (playerDetails.MunicipalityTeam == null)
            {
                return BadRequest(SharedHelper.CreateErrorResponse("Municipality's team not found"));
            }

            if (playerDetails.ExistingPersonTeam != null)
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse("Player is already part of this team")
                );
            }

            if (
                playerDetails.PlayerTeam?.Ward != null
                && playerDetails.PlayerTeam.Ward.MunicipalityId != munId
            )
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse("Player is not under our wards (Municipality)")
                );
            }

            // Assign the player to the district team
            var newPersonTeam = new PersonTeam
            {
                PersonId = playerId,
                TeamId = playerDetails.MunicipalityTeam.TeamId,
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
