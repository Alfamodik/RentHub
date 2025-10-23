using Microsoft.AspNetCore.Mvc;
using RentHub.API.Services;

namespace RentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SutochnoController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await SutochnoParser.Get());
        }
    }
}
