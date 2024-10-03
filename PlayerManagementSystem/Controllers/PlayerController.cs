using Microsoft.AspNetCore.Mvc;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.EfContext;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.Helpers;
namespace PlayerManagementSystem.Models
{
    public class PlayerController : ControllerBase
    {
        private readonly EfDbContext _context;

        public PlayerController(EfDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("players")]
        public async Task<IActionResult> GetAllPlayers()
        {
            try
            {
                var players = await _context.PersonalDetails.Where(p => p.Role.RoleName.Equals("player", StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
                var toReturn = new ApiResponse<List<PersonalDetails>>
                {
                    Data = players
                };
                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message ,  message = "Failed"});
            }
        }

        [HttpGet]
        [Route("coaches")]
        public async Task<IActionResult> GetAllCoaches()
        {
            //wrapping the code in a try catch block to handle any exceptions
            //no comments were added to the code

            try
            {
                var coaches = await _context.PersonalDetails.Where(p => p.Role.RoleName.Equals("coach", StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
                var toReturn = new ApiResponse<List<PersonalDetails>>();
                toReturn.Data = coaches;
                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message ,  message = "Failed"});
            }
            
        }

        [HttpGet]
        [Route("managers")]
        public async Task<IActionResult> GetAllManagers()
        {
            //wrapping the code in a try catch block to handle any exceptions
            //no comments were added to the code

            try
            {
                var managers = await _context.PersonalDetails.Where(p => p.Role.RoleName.Equals("manager", StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
                var toReturn = new ApiResponse<List<PersonalDetails>>();
                toReturn.Data = managers;
                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Error = ex.Message ,  message = "Failed"});
            }
        }

        [HttpGet]
        [Route("wardTeams/{id}")]
        public async Task<IActionResult> GetWardTeams(int id)
        {
            var teams = await _context.PersonalDetails.Where(t => t.TeamId == id).ToListAsync();
            var toReturn = new ApiResponse<List<PersonalDetails>>();
            toReturn.Data = teams;
            return Ok(toReturn);
        }
        

    }
}
    