using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagementSystem.EfContext;
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
                .PersonalDetails.Include(p => p.Role)
                .Include(details => details.Addresses)
                .Include(e => e.Team)
                .ToListAsync();

            return Ok(personalDetails);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(PersonalDetails personalDetails)
        {
            await _context.PersonalDetails.AddAsync(personalDetails);
            await _context.SaveChangesAsync();
            return Ok(personalDetails);
        }
    }
}
