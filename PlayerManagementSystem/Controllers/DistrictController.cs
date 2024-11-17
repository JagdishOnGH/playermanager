using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.DTOs;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistrictController(EfDbContext context) : ControllerBase
{
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllDistricts()
    {
        try
        {
            var districts = await context.Districts.Include(d => d.Province).ToListAsync();
            var toReturn = new ApiResponse<List<District>> { Data = districts };
            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = new ApiResponse<string> { Error = e.Message };
            return NotFound(error);
        }
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddDistrict(DistrictDto district)
    {
        try
        {
            //with province id check if province exists
            var province = await context.Provinces.FirstOrDefaultAsync(x =>
                x.ProvinceId == district.ProvinceId
            );
            if (province == null)
            {
                var error = new ApiResponse<string> { Error = "Province does not exist" };
                return NotFound(error);
            }
            //also under same province check if district name or id exists
            var districtExistsByName = await context.Districts.FirstOrDefaultAsync(x =>
                x.Name.ToUpper() == district.DistrictName.ToUpper()
            );
            if (districtExistsByName != null)
            {
                var error = new ApiResponse<string> { Error = "District already exists" };
                return BadRequest(error);
            }

            var newDistrict = new District
            {
                DistrictId = Guid.NewGuid(),
                ProvinceId = district.ProvinceId,
                Name = district.DistrictName.ToUpper(),
            };
            context.Teams.Add(
                new Team
                {
                    TeamId = Guid.NewGuid(),
                    Name = $"{newDistrict.Name}'s Team",
                    TerritoryId = newDistrict.DistrictId,
                    TerritoryType = TerritoryType.District,
                }
            );
            await context.Districts.AddAsync(newDistrict);
            await context.SaveChangesAsync();

            return Ok(newDistrict);
        }
        catch (Exception e)
        {
            var error = new ApiResponse<string> { Error = e.Message };
            return NotFound(error);
        }
    }
}
