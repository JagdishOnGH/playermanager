
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
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
    public async Task<IActionResult> GetAllProvince() {
        try
        {
            var provinces = await context.Provinces.ToListAsync();
          var toReturn  = new ApiResponse<List<Province>>
          {
              Data = provinces,
          };
            return Ok(toReturn);
          
        }
        catch (Exception e)
        {
            var error = new ApiResponse<string>
            {
                Error = e.Message
            };
            return NotFound(error);



        }
        
    }
    //
}