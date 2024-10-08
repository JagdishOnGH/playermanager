using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.Models.Auth;

namespace PlayerManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PalikaController(EfDbContext context, IConfiguration config) : ControllerBase
{
    private readonly IConfiguration _config = config;

    // Register Endpoint
    
    //all palikas
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllPalikas()
    {
        try
        {
            var palikas = await context.Palikas.ToListAsync();
            return Ok(new ApiResponse<List<Palika>> { Data = palikas });
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse<string> { Error = e.Message });
        }
    }
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterPalika(string palikaName)
    {
        try
        {
            var palika = new Palika
            {
                Name = palikaName,
                isLoginAssigned = false,
                Teams = [],
                Wards = []
            };
            await context.Palikas.AddAsync(palika);
            await context.SaveChangesAsync();
            return Ok(new ApiResponse<Palika> { Data = palika });

        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse<string> { Error = e.Message });
        }


    }

    [Authorize]
    [HttpPost]
    [Route("wards/add")]
    public async Task<IActionResult> AddWardsToPalika(Ward ward)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                
                
            }
            
            
            
            
            if (!User.HasClaim(ClaimTypes.Role, "PalikaAdmin"))
            {
                return Unauthorized(new ApiResponse<string>
                    { Error = "You are not authorized to perferm this action" });

            }
            var  palikaId = User.Claims.First(c => c.Type == "uuid").Value;
            var palika = await context.Palikas.Include(p=>p.Wards).FirstOrDefaultAsync(p => p.PalikaId == Guid.Parse(palikaId));
          if (palika == null)
          {
              return BadRequest(new ApiResponse<string> { Error = "Palika not found" });
          }
          palika.Wards.Add(ward);
            context.Palikas.Update(palika);
          await  context.SaveChangesAsync();
            return Ok(new ApiResponse<Palika> { Data = palika });


        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            
            return StatusCode(500, new ApiResponse<string>{Error = e.Message});
        }
    }
   
}
    