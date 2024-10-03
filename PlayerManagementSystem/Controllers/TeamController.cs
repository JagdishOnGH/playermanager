using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers
{
    public class TeamController : ControllerBase
    {
        private readonly EfDbContext _context;

        public TeamController(EfDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _context.Teams.ToListAsync();
            var toReturn = new ApiResponse<List<Teams>>();
            toReturn.Data = teams;
            return Ok(toReturn);
        }

        [HttpGet]
        [Route("filter")]
        public async Task<IActionResult> GetAll([FromQuery] int? teamId)
        {
            try
            {
                var query = _context.Teams.AsQueryable();

                if (teamId.HasValue)
                {
                    query = query.Where(p => p.TeamId == teamId.Value);
                }

                var teams = await query.ToListAsync();
                var toReturn = new ApiResponse<List<Teams>>
                {
                    Data = teams
                };
                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Teams team)
        {
            try
            {
                _context.Teams.Add(team);
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<Teams> { Data = team });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message });
            }
        }
    }
}