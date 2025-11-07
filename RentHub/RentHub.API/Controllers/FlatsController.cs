using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FlatsController : ControllerBase
    {
        [Authorize]
        [HttpGet("flats")]
        public ActionResult<List<Flat>> GetFlats()
        {
            using RentHubContext context = new();
            List<Flat> FlatsList = context.Flats.ToList();
            if (FlatsList.IsNullOrEmpty())
            {
                return NotFound("Список квартир пустой");
            }
            return FlatsList;
        }

        [Authorize]
        [HttpGet("flat-by-id/{id}")]
        public ActionResult<Flat> GetFlat(int id)
        {
            using RentHubContext context = new();
            Flat? flat = context.Flats.FirstOrDefault(fl => fl.FlatId == id);

            if (flat == null)
            {
                return NotFound($"Квартира с ID {id} не найдена");
            }

            return Ok(flat);
        }

        [Authorize]
        [HttpGet("user-flats")]
        public ActionResult<List<Flat>> GetUserFlats()
        {
            using RentHubContext context = new();
            string? claimedUserid = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimedUserid))
            {
                return Unauthorized("Не найден токен");
            }

            if (!int.TryParse(claimedUserid, out int userId))
            {
                return Unauthorized("Неверный формат токена");
            }

            List<Flat> flats = context.Flats.Where(fl => fl.UserId == userId).ToList();

            if (flats.IsNullOrEmpty())
            {
                return NotFound($"Квартиры пользователя с ID {userId} не найдены");
            }

            return Ok(flats);
        }

        [Authorize]
        [HttpPost("flat")]
        public ActionResult AddFlat(FlatDTO flatDto)
        {
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
                UserId = flatDto.UserId,
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

            return Ok("Квартира успешно добавлена");
        }

        [Authorize]
        [HttpPut("flat-data/{id}")]
        public ActionResult ChangeFlatData(int id, FlatDTO flatDto)
        {
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
            Flat? flat = context.Flats.FirstOrDefault(fl => fl.FlatId == id);

            if (flat == null)
            {
                return NotFound($"Квартира с ID {id} не найдена");
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
            flat.Photo = photoBytes;
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
