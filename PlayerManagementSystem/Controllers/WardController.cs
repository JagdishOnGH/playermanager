using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.DTOs;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]

public class WardController(EfDbContext context): ControllerBase
{
     [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllMunicipals()
    {
        try
        {
            var wards = await context.Wards.Include(w=>w.Municipality).ToListAsync();
            var toReturn = new ApiResponse<List<Ward>>
            {
                Data = wards,
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
    
    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddDistrict(WardDto ward)
    {
        try
        {
            //with province id check if province exists
            var municipal = await context.Municipalities.FirstOrDefaultAsync(x => x.MunicipalityId == ward.MunicipalityId);
            if (municipal == null)
            {
                var error = new ApiResponse<string>
                {
                    Error = $"Municipality does not exist with id {ward.MunicipalityId} "
                };
                return NotFound(error);
            }
            //also under same province check if district name or id exists
            var wardNoExist = await context.Wards.FirstOrDefaultAsync(x =>
                x.WardNo.ToUpper() == ward.WardNo.ToUpper() );
            if (wardNoExist != null)
            {
                var error = new ApiResponse<string>
                {
                    Error = $"Ward No  already exists with {ward.WardNo}"
                };
                return BadRequest(error);
            }


            
            
            
            

            var newWard = new Ward
            {
                WardId = Guid.NewGuid(),
                MunicipalityId = ward.MunicipalityId,
                WardNo = ward.WardNo
            };
            await context.Wards.AddAsync(newWard);
            await context.SaveChangesAsync();

            return Ok(newWard);

          

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
    
    
}