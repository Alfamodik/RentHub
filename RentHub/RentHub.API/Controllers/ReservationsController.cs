using Microsoft.AspNetCore.Mvc;
using RentHub.Core.Model;

namespace RentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        [HttpGet("GetReservations")]
        public ActionResult Get()
        {
            return Ok(RenthubContext.Instance.Reservations.ToList());
        }
    }
}
