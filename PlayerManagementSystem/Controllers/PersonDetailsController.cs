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
    public async Task<IActionResult> AddPersonDetails(PersonDetailsDto personDetails)
    {
        try
        {
            var person = new Person
            {
                FirstName = personDetails.FirstName,
                LastName = personDetails.LastName,
                Role = personDetails.Role,
                Email = personDetails.Email,
            };
            
            var teamId = personDetails.TeamId;
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
            
            
         await   SharedHelper.ValidateTeamRolesAsync(personDetails.TeamId,personDetails.Role, context);
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

