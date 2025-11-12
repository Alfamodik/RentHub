using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.API.Services;
using RentHub.Core.Model;
using System;
using System.Security.Claims;

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
            return context.Reservations.ToList();
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

        [HttpGet("reservation-by-flat-id/{id}")]
        public ActionResult<Reservation> GetAdvertisementOfFlat(int id)
        {
            using RentHubContext context = new();
            List<Reservation> reservations = context.Reservations
                 .Include(r => r.Advertisement)
                 .Where(r => r.Advertisement.FlatId == id)
                 .ToList();

            return Ok(reservations);
        }

        [Authorize]
        [HttpPost("reservation")]
        public ActionResult AddReservation(ReservationDTO reservationdto)
        {
            using RentHubContext context = new();
            bool hasOverlap = context.Reservations
                .Where(r => r.AdvertisementId == reservationdto.AdvertisementId)
                .Any(r => (reservationdto.DateOfStartReservation <= r.DateOfEndReservation &&
                  reservationdto.DateOfEndReservation >= r.DateOfStartReservation));

            if (hasOverlap)
            {
                return BadRequest("Период бронирования пересекается с существующими бронированиями");
            }

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

        [Authorize]
        [HttpGet("update")]
        public async Task<IActionResult> UpdateAsync()
        {
            string? claimedUserid = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claimedUserid, out int userId))
                return Unauthorized();

            RentHubContext context = new();

            List<Flat>? flats = context.Flats
                .Include(flat => flat.Advertisements)
                .ThenInclude(advertisiment => advertisiment.Platform)
                .Where(flat => flat.UserId == userId)
                .ToList();

            foreach (Flat flat in flats)
            {
                foreach (Advertisement advertisiment in flat.Advertisements)
                {
                    switch (advertisiment.Platform.PlatformName)
                    {
                        case "Avito":



                            break;

                        case "Sutochno":
                            List<Reservation>? reservations = await SutochnoParser.GetReservations(advertisiment.LinkToAdvertisement);

                            if (reservations == null)
                                break;

                            foreach (Reservation reservation in reservations)
                            {
                                if (context.Reservations
                                    .Any(reservation =>
                                    reservation.DateOfStartReservation == reservation.DateOfStartReservation
                                    || reservation.DateOfEndReservation == reservation.DateOfEndReservation))
                                {
                                    continue;
                                }

                                int days = (
                                    reservation.DateOfEndReservation.ToDateTime(TimeOnly.MinValue) -
                                    reservation.DateOfStartReservation.ToDateTime(TimeOnly.MinValue)
                                    ).Days;

                                reservation.AdvertisementId = advertisiment.AdvertisementId;
                                reservation.Summ = days * advertisiment.PriceForPeriod;
                                reservation.Income = days * advertisiment.IncomeForPeriod;
                                context.Reservations.Add(reservation);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            return NoContent();
        }
    }
}
