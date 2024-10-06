using Microsoft.AspNetCore.Mvc;

namespace PlayerManagementSystem.Controllers
{
    [ApiController]
    public class WardController : ControllerBase
    {
        public async Task<IActionResult> GetAllWards ()
        {

            return Ok("hi");
            //TODO to implement


        }
        
    }
}

