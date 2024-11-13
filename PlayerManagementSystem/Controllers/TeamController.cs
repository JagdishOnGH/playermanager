using Microsoft.AspNetCore.Mvc;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TeamController : ControllerBase
{
    public Task<IActionResult> CreateTeams(Team team)
    {
        throw new Exception(
        );
    }
    //
}