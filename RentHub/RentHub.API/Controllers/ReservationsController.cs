using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.API.RequestModels.Avito;
using RentHub.API.Services;
using RentHub.Core.Model;
using System;
using System.ComponentModel.DataAnnotations;
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
            if (!reservationdto.AdvertisementId.HasValue)
                return BadRequest("ID объявления обязателен");

            if (!reservationdto.RenterId.HasValue)
                return BadRequest("ID арендателя обязателен");

            if (!reservationdto.DateOfStartReservation.HasValue)
                return BadRequest("Дата начала бронирования обязательна");

            if (!reservationdto.DateOfEndReservation.HasValue)
                return BadRequest("Дата окончания бронирования обязательна");

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
                AdvertisementId = reservationdto.AdvertisementId.Value,
                RenterId = reservationdto.RenterId.Value,
                DateOfStartReservation = reservationdto.DateOfStartReservation.Value,
                DateOfEndReservation = reservationdto.DateOfEndReservation.Value,
                Summ = reservationdto.Summ ?? 0,
                Income = reservationdto.Income ?? 0
            };
            context.Reservations.Add(reservation).Context.SaveChanges();
            return Ok("Бронирование успешно добавлено");
        }

        //[Authorize]
        [HttpPut("reservation-data/{id}")]
        public ActionResult ChangeReservationData(int id, ReservationDTO reservationDTO)
        {
            using RentHubContext context = new();
            Reservation? reservation = context.Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (reservation == null)
            {
                return NotFound($"Бронирование с ID {id} не найдено");
            }
            if (reservationDTO.AdvertisementId.HasValue && reservationDTO.AdvertisementId != 0)
                reservation.AdvertisementId = reservationDTO.AdvertisementId.Value;

            if (reservationDTO.RenterId.HasValue && reservationDTO.RenterId != 0)
                reservation.RenterId = reservationDTO.RenterId.Value;

            if (reservationDTO.DateOfEndReservation.HasValue && reservationDTO.DateOfEndReservation > DateOnly.MinValue)
                reservation.DateOfEndReservation = reservationDTO.DateOfEndReservation.Value;

            if (reservationDTO.DateOfStartReservation.HasValue && reservationDTO.DateOfStartReservation.Value > DateOnly.MinValue)
                reservation.DateOfStartReservation = reservationDTO.DateOfStartReservation.Value;

            if (reservationDTO.Summ.HasValue && reservationDTO.Summ != 0)
                reservation.Summ = reservationDTO.Summ.Value;

            if (reservationDTO.Income.HasValue && reservationDTO.Income != 0)
                reservation.Income = reservationDTO.Income.Value;
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
            User? user = context.Users.FirstOrDefault(user => user.UserId == userId);

            if (user == null)
                return Unauthorized();

            List<Flat>? flats = context.Flats
                .Include(flat => flat.Advertisements)
                .ThenInclude(advertisiment => advertisiment.Platform)
                .Where(flat => flat.UserId == userId)
                .ToList();

            List<Reservation>? reservations = null;

            foreach (Flat flat in flats)
            {
                List<Reservation> existingReservations = context.Reservations
                    .Where(r => r.Advertisement.FlatId == flat.FlatId)
                    .ToList();

                List<Reservation> newReservations = new();

                foreach (Advertisement advertisiment in flat.Advertisements)
                {
                    switch (advertisiment.Platform.PlatformName)
                    {
                        case "Avito":
                            reservations = await AvitoServices.GetReservations(user, advertisiment.LinkToAdvertisement);
                            break;

                        case "Sutochno":
                            reservations = await SutochnoParser.GetReservations(advertisiment.LinkToAdvertisement);
                            break;

                        default:
                            continue;
                    }

                    if (reservations == null)
                        continue;

                    foreach (Reservation reservation in reservations)
                    {
                        if (context.Reservations
                            .Where(item => item.Advertisement.FlatId == flat.FlatId)
                            .ToList()
                            .Concat(newReservations)
                            .Any(existing =>
                            !(existing.DateOfEndReservation <= reservation.DateOfStartReservation ||
                            reservation.DateOfEndReservation <= existing.DateOfStartReservation)))
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
                        
                        newReservations.Add(reservation);
                        context.Reservations.Add(reservation);
                    }
                }
            }

            context.SaveChanges();
            return NoContent();
        }
    }
}
