using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class WardController(EfDbContext context) : ControllerBase
    {
        [Route("all")]
        [HttpGet]
        public async Task<IActionResult> GetAllWards ()
        {

            try
            {
                var data = await context.Wards.ToListAsync();
                return Ok(new ApiResponse<List<Ward>> { Data = data });

            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiResponse<string>
                    {
                        Error = e.Message,
                        message = "Failed"
                        
                    })
                ;
            }


        }
        
    }
}

