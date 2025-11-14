using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FlatsController : ControllerBase
    {
        [Authorize]
        [HttpGet("flats")]
        public ActionResult<IEnumerable<Flat>> GetFlats()
        {
            using RentHubContext context = new();
            return context.Flats.ToList();
        }

        [Authorize]
        [HttpGet("flat-by-id/{id}")]
        public ActionResult<Flat> GetFlat(int id)
        {
            using RentHubContext context = new();
            return Ok(context.Flats.FirstOrDefault(fl => fl.FlatId == id));
        }

        [Authorize]
        [HttpGet("user-flats")]
        public ActionResult<IEnumerable<Flat>> GetUserFlats()
        {
            using RentHubContext context = new();
            string? claimedUserid = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claimedUserid, out int userId))
            {
                return Unauthorized("Неверный формат токена");
            }

            IEnumerable<Flat> flats = context.Flats
                .Where(flat => flat.UserId == userId)
                .Include(flat => flat.Advertisements)
                .ThenInclude(advertisement => advertisement.Reservations)
                .ThenInclude(reservation => reservation.Renter)
                .Include(flat => flat.Advertisements)
                .ThenInclude(advertisement => advertisement.Platform)
                .ToList();

            return Ok(flats);
        }

        [Authorize]
        [HttpPost("flat")]
        public ActionResult AddFlat(FlatDTO flatDto)
        {
            string? claimedUserid = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claimedUserid, out int userId))
            {
                return Unauthorized("Неверный формат токена");
            }
            using RentHubContext context = new();
            byte[]? photoBytes = null;
            if (flatDto.Photo != null)
            {
                // Проверка размера файла (до 5MB)
                if (flatDto.Photo.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("Размер файла не должен превышать 5MB");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(flatDto.Photo.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Допустимые форматы: JPG, JPEG, PNG, GIF");
                }

                using var memoryStream = new MemoryStream();
                flatDto.Photo.CopyToAsync(memoryStream);
                photoBytes = memoryStream.ToArray();
                memoryStream.Close();
            }
            var flat = new Flat
            {
                UserId = userId,
                Country = flatDto.Country,
                City = flatDto.City,
                District = flatDto.District,
                HouseNumber = flatDto.HouseNumber,
                ApartmentNumber = flatDto.ApartmentNumber,
                RoomCount = flatDto.RoomCount,
                Size = flatDto.Size,
                FloorNumber = flatDto.FloorNumber,
                FloorsNumber = flatDto.FloorsNumber,
                Description = flatDto.Description,
                Photo = photoBytes
            };

            context.Flats.Add(flat);
            context.SaveChanges();

            var add = new Advertisement
            {
                FlatId = flat.FlatId,
                PlatformId = 3,
                RentType = "Посуточно",
                PriceForPeriod = 500,
                IncomeForPeriod = 500
            };
            context.Advertisements.Add(add);
            context.SaveChanges();
            return Ok("Квартира успешно добавлена");
        }

        [Authorize]
        [HttpPut("flat-data/{id}")]
        public async Task<ActionResult> ChangeFlatData(int id, [FromForm] FlatDTO flatDto)
        {
            using RentHubContext context = new();

            Flat? flat = context.Flats.FirstOrDefault(fl => fl.FlatId == id);

            if (flat == null)
            {
                return NotFound($"Квартира с ID {id} не найдена");
            }

            if (flatDto.Photo != null)
            {
                byte[]? photoBytes = null;

                // Проверка размера файла (до 5MB)
                if (flatDto.Photo.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("Размер файла не должен превышать 5MB");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(flatDto.Photo.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Допустимые форматы: JPG, JPEG, PNG, GIF");
                }

                using var memoryStream = new MemoryStream();
                await flatDto.Photo.CopyToAsync(memoryStream);
                photoBytes = memoryStream.ToArray();
                flat.Photo = photoBytes;
            }
            
            flat.Country = flatDto.Country;
            flat.City = flatDto.City;
            flat.District = flatDto.District;
            flat.HouseNumber = flatDto.HouseNumber;
            flat.RoomCount = flatDto.RoomCount;
            flat.Size = flatDto.Size;
            flat.FloorNumber = flatDto.FloorNumber;
            flat.FloorsNumber = flatDto.FloorsNumber;
            flat.Description = flatDto.Description;
            context.SaveChanges();

            return Ok("Данные квартиры успешно изменены");
        }

        [Authorize]
        [HttpDelete("flat/{id}")]
        public ActionResult DeleteFlat(int id)
        {
            using RentHubContext context = new();
            Flat? flat = context.Flats.FirstOrDefault(fl => fl.FlatId == id);

            if (flat == null)
            {
                return NotFound($"Квартира с ID {id} не найдена");
            }
            context.Flats.Remove(flat);
            context.SaveChanges();
            return Ok("Квартира успешно удалена");
        }
    }
}
