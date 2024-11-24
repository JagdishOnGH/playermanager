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
public class PersonDetailsController(EfDbContext context) : ControllerBase
{
    [HttpPost]
    [Route("add")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> AddPersonDetails(PersonDetailsDto personDetails)
    {
        try
        {
            if(!User.HasClaim("Role", "Ward"))
            {
             //   var allClaimsinString = User.Claims.Select(x => x.Value).Aggregate((x, y) => x + " " + y);  
                
                var error = SharedHelper.CreateErrorResponse("You are not ward admin to perform this action ");
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
            var person = new Person
            {
                FirstName = personDetails.FirstName,
                LastName = personDetails.LastName,
                Role = personDetails.Role,
                Email = personDetails.Email,
            };
            
            var teamId = myTeam.TeamId;
            var team = await context.Teams.FirstOrDefaultAsync(x => x.TeamId == teamId );
            if (team == null)
            {
                var error = new ApiResponse<string> { Error = "Team does not exist" };
                return NotFound(error);
            }

            if (team.TerritoryType != TerritoryType.Ward)
            {
                var error = new ApiResponse<string> { Error = "Player can only be added by ward" };
                return BadRequest(error);
            }

            var personTeam = new PersonTeam
            {
                Person = person,
                Team = team,
            };
            
            
         await   SharedHelper.ValidateTeamRolesAsync(teamId,personDetails.Role, context);
            await context.PersonTeams.AddAsync(personTeam);
             

            await context.Persons.AddAsync(person);
            await context.SaveChangesAsync();
            var toReturn = new ApiResponse<Person> { Data = person };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = new ApiResponse<string> { Error = e.Message };
            return BadRequest(error);
        }
    }
     


}

