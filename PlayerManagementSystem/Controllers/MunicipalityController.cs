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
public class MunicipalityController(EfDbContext context) : ControllerBase
{
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllMunicipals()
    {
        try
        {
            var districts = await context.Municipalities.Include(d => d.District).ToListAsync();
            var toReturn = new ApiResponse<List<Municipality>> { Data = districts };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse(e.Message);
            return NotFound(error);
        }
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddDistrict(MunicipalsDto municipals)
    {
        try
        {
            //with province id check if province exists
            var province = await context.Districts.FirstOrDefaultAsync(x =>
                x.DistrictId == municipals.DistrictId
            );
            if (province == null)
            {
                var error = SharedHelper.CreateErrorResponse(
                    $"District does not exist with id {municipals.DistrictId} "
                );
                return NotFound(error);
            }
            //also under same province check if district name or id exists
            var municipalExistsByName = await context.Municipalities.FirstOrDefaultAsync(x =>
                x.Name.ToUpper() == municipals.MunipalityName.ToUpper()
            );
            if (municipalExistsByName != null)
            {
                // var error = new ApiResponse<string> { Error = "Municipality already exists" };
                var error = SharedHelper.CreateErrorResponse("Municipality already exists");
                return BadRequest(error);
            }

            var newMun = new Municipality
            {
                MunicipalityId = Guid.NewGuid(),
                DistrictId = municipals.DistrictId,
                Name = municipals.MunipalityName.ToUpper(),
            };
            context.Teams.Add(
                new Team
                {
                    TeamId = Guid.NewGuid(),
                    Name = $"{municipals.MunipalityName}'s Team",
                    TerritoryId = newMun.MunicipalityId,
                    TerritoryType = TerritoryType.Municipality,
                }
            );
            await context.Municipalities.AddAsync(newMun);
            await context.SaveChangesAsync();

            return Ok(newMun);
        }
        catch (Exception e)
        {
            //       var error = new ApiResponse<string> { Error = e.Message };
            var error = SharedHelper.CreateErrorResponse(e.Message);
            return NotFound(error);
        }
    }
}
