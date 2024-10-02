using Microsoft.AspNetCore.Mvc;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.Repositories;

namespace PlayerManagementSystem.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly PlayerRepository _playerRepository;
    private readonly EfDbContext _context;
    

    public PlayerController(EfDbContext context)
    {
        _context = context;
        _playerRepository = new PlayerRepository(_context);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Player>>>> GetAllPlayers()
    {
        var players = await _playerRepository.GetAllPlayers();
        return Ok(new ApiResponse<List<Player>> { Data = players });
    }
    
}