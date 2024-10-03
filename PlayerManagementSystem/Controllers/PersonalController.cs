using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PersonalController(EfDbContext context) : ControllerBase
    {
        private readonly EfDbContext _context = context;

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            var personalDetails = await _context
                .PersonalDetails
                .Include(p => p.Role)
                .Include(details => details.Addresses)
                .Include(e => e.Team)
                .ToListAsync();
            var toReturn = new ApiResponse<List<PersonalDetails>>();
            toReturn.Data = personalDetails;
            return Ok(toReturn);
        }
        
        [HttpGet]
        [Route("filter")]
        public async Task<IActionResult> GetAll([FromQuery] int? ward, [FromQuery] int? teamId)
        {
            try
            {
                var query = _context.PersonalDetails
                    .Include(p => p.Role)
                    .Include(details => details.Addresses)
                    .Include(e => e.Team)
                    .AsQueryable();

                if (ward.HasValue)
                {
                    query = query.Where(p => p.Addresses.Any(a => a.Ward == ward.Value));
                }

                if (teamId.HasValue)
                {
                    query = query.Where(p => p.TeamId == teamId.Value);
                }

                var personalDetails = await query.ToListAsync();
                var toReturn = new ApiResponse<List<PersonalDetails>>
                {
                    Data = personalDetails
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
        public async Task<IActionResult> Add(PersonalDetails personalDetails)
        {
            await _context.PersonalDetails.AddAsync(personalDetails);
            await _context.SaveChangesAsync();
            var toReturn = new ApiResponse<PersonalDetails>
            {
                Data = personalDetails
            };
            return Ok(personalDetails);
        }

        [HttpDelete]
        [Route("delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var personalDetails = await _context.PersonalDetails.Include(p => p.Role)
                .Include(details => details.Addresses)
                .Include(e => e.Team)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (personalDetails == null)
            {
                return NotFound();
            }

            _context.Addresses.RemoveRange(personalDetails.Addresses);

            _context.PersonalDetails.Remove(personalDetails);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
