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

    [HttpPost]
    [Route("add-person")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> AddPerson([FromBody]Guid playerId)
    {
        try
        {
            // Verify user role
            if (!User.HasClaim("Role", "District"))
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse(
                        "You are not authorized to perform this action"
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
            var tokenDistrictId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (string.IsNullOrEmpty(tokenDistrictId))
            {
                return BadRequest(SharedHelper.CreateErrorResponse("District not found in token"));
            }

            var districtId = Guid.Parse(tokenDistrictId);

            // Consolidate data retrieval in a single query using projections
            var playerDetails = await context
                .Persons.Where(p => p.PersonId == playerId)
                .Select(p => new
                {
                    Player = p,
                    DistrictTeam = context.Teams.FirstOrDefault(t => t.TerritoryId == districtId),
                    ExistingPersonTeam = context.PersonTeams.FirstOrDefault(pt =>
                        pt.PersonId == playerId && pt.Team.TerritoryId == districtId
                    ),
                    PlayerTeam = context
                        .PersonTeams.Where(pt =>
                            pt.PersonId == playerId
                            && pt.Team.TerritoryType == TerritoryType.Municipality
                        )
                        .Select(pt => new
                        {
                            Team = pt.Team,
                            Municipality = context.Municipalities.FirstOrDefault(m =>
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

            if (playerDetails.DistrictTeam == null)
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
                playerDetails.PlayerTeam?.Municipality != null
                && playerDetails.PlayerTeam.Municipality.DistrictId != districtId
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
                TeamId = playerDetails.DistrictTeam.TeamId,
            };

            context.PersonTeams.Add(newPersonTeam);
            await context.SaveChangesAsync();

            return Ok(new ApiResponse<Person> { Data = playerDetails.Player });
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse("Something went wrong: " + e.Message);
            return StatusCode(500, error);
        }
    }

    [HttpGet("available-to-add")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetAvailablePlayers()
    {
        try
        {
            // Verify user role
            if (!User.HasClaim("Role", "District"))
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse(
                        "You are not authorized to perform this action"
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
            var tokenDistrictId = User.Claims.FirstOrDefault(x => x.Type == "TerritoryId")?.Value;
            if (string.IsNullOrEmpty(tokenDistrictId))
            {
                return BadRequest(SharedHelper.CreateErrorResponse("District not found in token"));
            }

            var districtId = Guid.Parse(tokenDistrictId);
            var municipalityPlayers =await context.Municipalities
                .Where(m => m.DistrictId == districtId) // Filter municipalities by district
                .Select(m => new
                {
                    MunicipalityName = m.Name, // Replace with the actual column/property for municipality name
                    Players =  context.Teams
                        .Where(t => t.TerritoryId == m.MunicipalityId)
                        .SelectMany(t => context.PersonTeams
                            .Where(pt => pt.TeamId == t.TeamId)
                            .Include(pt => pt.Person)
                            .Include(pt => pt.Team)
                            .Select(pt=> new
                            {
                                name=pt.Person.FirstName+" "+pt.Person.LastName,
                                role=pt.Person.Role.ToString(),
                                team=pt.Team.Name,
                                teamId=pt.Team.TeamId
                            })
                        ) // Select Player IDs
                        .Distinct() // Remove duplicate players in this municipality
                        .ToList() // Convert players to a list
                })
                .ToListAsync(); // Convert the result to a list of municipalities

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
class SomethingException(string message) : Exception;
