using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentHub.Core.Model;

namespace RentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        [Authorize]
        [HttpGet("GetReservations")]
        public ActionResult Get()
        {
            return Ok(RentHubContext.Instance.Reservations.ToList());
        }
    }
}
