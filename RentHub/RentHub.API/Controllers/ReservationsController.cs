using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        [Authorize]
        [HttpGet("reservations")]
        public ActionResult<List<Reservation>> GetReservations()
        {
            using RentHubContext context = new();
            List<Reservation> ReservationsList = context.Reservations.ToList();
            if (ReservationsList.IsNullOrEmpty())
            {
                return NotFound("Список бронирований пуст");
            }
            return ReservationsList;
        }

        [Authorize]
        [HttpGet("reservation-by-id/{id}")]
        public ActionResult<Reservation> GetReservation(int id)
        {
            using RentHubContext context = new();
            Reservation? reservation = context.Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
            {
                return NotFound($"Бронирование с ID {id} не найден");
            }
            return Ok(reservation);
        }

        [Authorize]
        [HttpPost("reservation")]
        public ActionResult AddReservation(ReservationDTO reservationdto)
        {
            using RentHubContext context = new();
            Reservation reservation = new Reservation
            {
                AdvertisementId = reservationdto.AdvertisementId,
                RenterId = reservationdto.RenterId,
                DateOfStartReservation = reservationdto.DateOfStartReservation,
                DateOfEndReservation = reservationdto.DateOfEndReservation,
                Summ = reservationdto.Summ,
                Income = reservationdto.Income
            };
            context.Reservations.Add(reservation).Context.SaveChanges();
            return Ok("Бронирование успешно добавлено");
        }

        [Authorize]
        [HttpPut("reservation-data/{id}")]
        public ActionResult ChangeReservationData(int id, ReservationDTO reservationDTO)
        {
            using RentHubContext context = new();
            Reservation? reservation = context.Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
            {
                return NotFound($"Бронирование с ID {id} не найдено");
            }
            reservation.AdvertisementId = reservationDTO.AdvertisementId;
            reservation.RenterId = reservationDTO.RenterId;
            reservation.DateOfEndReservation = reservationDTO.DateOfEndReservation;
            reservation.DateOfStartReservation = reservationDTO.DateOfStartReservation;
            reservation.Summ = reservationDTO.Summ;
            reservation.Income = reservationDTO.Income;
            context.SaveChanges();

            return Ok("Данные бронирования успешно изменены");
        }

        [Authorize]
        [HttpDelete("reservation/{id}")]
        public ActionResult DeleteReservation(int id)
        {
            using RentHubContext context = new();
            Reservation? reservation = context.Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
            {
                return NotFound($"Бронирование с ID {id} не найдено");
            }
            context.Reservations.Remove(reservation).Context.SaveChanges();
            return Ok("Бронирование успешно удалено");
        }
    }
}
