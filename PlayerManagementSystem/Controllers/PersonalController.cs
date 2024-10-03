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

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(PersonalDetails personalDetails)
        {
            await _context.PersonalDetails.AddAsync(personalDetails);
            await _context.SaveChangesAsync();
            var toReturn = new ApiResponse<PersonalDetails>();
            toReturn.Data = personalDetails;
            return Ok(personalDetails);
        }

        [HttpPut]
        [Route("update/{id}")]

        public async Task<IActionResult> Update(int id, PersonalDetails personalDetails)
        {
            try{
            var personalDetailsToUpdate = await _context.PersonalDetails.Include(p => p.Role)
                .Include(details => details.Addresses)
                .Include(e => e.Team)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (personalDetailsToUpdate == null)
            {
                return NotFound();
            }

            personalDetailsToUpdate.Name = personalDetails.Name;
            personalDetailsToUpdate.ProfilePicUrl = personalDetails.ProfilePicUrl;
            personalDetailsToUpdate.PhoneNo = personalDetails.PhoneNo;
            personalDetailsToUpdate.Email = personalDetails.Email;
            personalDetailsToUpdate.Dob = personalDetails.Dob;
            personalDetailsToUpdate.TeamId = personalDetails.TeamId;

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<PersonalDetails>{
                Data = personalDetailsToUpdate
            });


            }
            catch (Exception ex){
                return BadRequest(new ApiResponse<PersonalDetails>{
                    message = "Failed",
                    Error = ex.Message
                });
            }
        }



        [HttpDelete]
        [Route("delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            try{
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

            return Ok(new ApiResponse<IEnumerable<PersonalDetails>>{
                message = "Deleted Successfully",
                Data = await _context.PersonalDetails.ToListAsync()
            });
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiResponse<PersonalDetails>{
                    message = "Failed to delete",
                    Error = ex.Message
                });
            }
        }
    }
}
