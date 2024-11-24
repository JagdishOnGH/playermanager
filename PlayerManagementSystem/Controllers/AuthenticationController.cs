using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.DTOs;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helper;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.Models.AuthModel;

namespace PlayerManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(
    UserManager<AppUser> userManager,
    EfDbContext context,
    JwtHelper helper
) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterTerrAdmin([FromBody] RegisterDto registerDto)
    {
        try
        {
            
            // Return BadRequest if model state is invalid
            if (!ModelState.IsValid)
            {
                // Log validation errors


                // Return a BadRequest with validation errors
                return BadRequest(
                    new
                    {
                        message = "Validation failed",
                        errors = ModelState
                            .Values.SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage),
                    }
                );
            }

            bool doesTerritoryExist = registerDto.Role switch
            {
                TerritoryType.Province => await context.Provinces.AnyAsync(x =>
                    x.ProvinceId == registerDto.TerritoryId
                ),

                TerritoryType.District => await context.Districts.AnyAsync(x =>
                    x.DistrictId == registerDto.TerritoryId
                ),
                TerritoryType.Municipality => await context.Municipalities.AnyAsync(x =>
                    x.MunicipalityId == registerDto.TerritoryId
                ),
                TerritoryType.Ward => await context.Wards.AnyAsync(x =>
                    x.WardId == registerDto.TerritoryId
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(registerDto.Role)),
            };

            if (!doesTerritoryExist)
            {
                return BadRequest(
                    SharedHelper.CreateErrorResponse(
                        "Territory does not exist, Check ID is matching with the territory type"
                    )
                );
            }

            var newUser = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                TerritoryId = registerDto.TerritoryId,
            };

            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                var error = SharedHelper.CreateErrorResponse("Passwords do not match");
                return BadRequest(error);
            }

            var result = await userManager.CreateAsync(newUser, registerDto.Password);

            if (result.Succeeded)
            {
                var role = registerDto.Role.ToString();
                await userManager.AddToRoleAsync(newUser, role);

                var toReturn = new Dictionary<string, string>()
                {
                    { "message", "User created successfully" },
                    { "userId", newUser.Id },
                };
                return Ok(toReturn);
            }

            // Instead of using ToString(), extract error messages from IdentityResult.Errors
            var errorMessages = result.Errors.Select(e => e.Description).ToList();
            var userCreationError = SharedHelper.CreateErrorResponse(
                string.Join(", ", errorMessages)
            );

            return BadRequest(userCreationError);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse(e.Message);
            return BadRequest(error);
        }
    }

    //loginwith email pass and get auth token and role and return
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(loginDto.emailAdress);
            if (user == null)
            {
                return BadRequest(SharedHelper.CreateErrorResponse("User does not exist"));
            }

            var result = await userManager.CheckPasswordAsync(user, loginDto.password);
            if (!result)
            {
                return BadRequest(SharedHelper.CreateErrorResponse("Invalid password"));
            }

            var roles = await userManager.GetRolesAsync(user);

            var token = helper.GenerateJwt(user, roles[0]);
            var myTeamId = context.Teams.FirstOrDefault(x => x.TerritoryId == user.TerritoryId)?.TeamId;
            if (myTeamId == null)
            {
                return NotFound(SharedHelper.CreateErrorResponse("Team not found"));
            }
            
            

            var toReturn = new Dictionary<string, object>
            {
                { "message", "Login successful" },
                { "userId", user.Id },
                { "token", token },
                {"teamId", myTeamId}
            };

            return Ok(toReturn);
        }
        catch (Exception e)
        {
            var error = SharedHelper.CreateErrorResponse(e.Message);
            return BadRequest(error);
        }
    }
}
