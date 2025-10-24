using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        [HttpGet("GetReservations")]
        public ActionResult<List<Reservation>> GetReservations()
        {
            List<Reservation> ReservationsList = RentHubContext.Instance.Reservations.ToList();
            if (ReservationsList.IsNullOrEmpty())
            {
                return NotFound("Список бронирований пуст");
            }
            return ReservationsList;
        }

        [HttpGet("GetReservationById{id}")]
        public ActionResult<Reservation> GetReservation(int id)
        {
            List<Reservation> ReservationsList = RentHubContext.Instance.Reservations.ToList();
            Reservation? reservation = ReservationsList.FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound($"Бронирование с ID {id} не найден");
            }

            return Ok(reservation);
        }

        [HttpPost("AddReservation")]
        public ActionResult AddReservation(ReservationDTO reservationdto)
        {
            Reservation reservation = new Reservation
            {
                AdvertisementId = reservationdto.AdvertisementId,
                RenterId = reservationdto.RenterId,
                DateOfStartReservation = reservationdto.DateOfStartReservation,
                DateOfEndReservation = reservationdto.DateOfEndReservation,
                Summ = reservationdto.Summ,
                Income = reservationdto.Income
            };
            RentHubContext.Instance.Reservations.Add(reservation).Context.SaveChanges();
            return Ok("Бронирование успешно добавлено");
        }

        [HttpPut("ChangeReservationData{id}")]
        public ActionResult ChangeReservationData(int id, ReservationDTO reservationDTO)
        {
            List<Reservation> ReservationsList = RentHubContext.Instance.Reservations.ToList();
            Reservation? reservation = ReservationsList.FirstOrDefault(r => r.ReservationId == id);
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
            RentHubContext.Instance.SaveChanges();

            return Ok("Данные бронирования успешно изменены");
        }

        [HttpDelete("DeleteReservation{id}")]
        public ActionResult DeleteReservation(int id)
        {
            List<Reservation> ReservationsList = RentHubContext.Instance.Reservations.ToList();
            Reservation? reservation = ReservationsList.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
            {
                return NotFound($"Бронирование с ID {id} не найдено");
            }
            RentHubContext.Instance.Reservations.Remove(reservation).Context.SaveChanges();
            return Ok("Бронирование успешно удалено");
        }
    }
}
