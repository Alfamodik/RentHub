using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlatsController : ControllerBase
    {
        [HttpGet("GetFlats")]
        public ActionResult<List<Flat>> GetFlats()
        {
            List<Flat> FlatsList = RentHubContext.Instance.Flats.ToList();
            if(FlatsList.IsNullOrEmpty())
            {
                return NotFound("Список квартир пустой");
            }
            return FlatsList;
        }

        [HttpGet("GetFlatById{id}")]
        public ActionResult<Flat> GetFlat(int id)
        {
            List<Flat> FlatsList = RentHubContext.Instance.Flats.ToList();
            Flat? flat = FlatsList.FirstOrDefault(fl => fl.FlatId == id);

            if (flat == null)
            {
                return NotFound($"Квартира с ID {id} не найдена");
            }

            return Ok(flat);
        }

        [HttpPost("AddFlat")]
        public ActionResult AddFlat(FlatDTO flatDto)
        {
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
                Country = flatDto.Country,
                City = flatDto.City,
                District = flatDto.District,
                HouseNumber = flatDto.HouseNumber,
                RoomCount = flatDto.RoomCount,
                Size = flatDto.Size,
                FloorNumber = flatDto.FloorNumber,
                FloorsNumber = flatDto.FloorsNumber,
                Description = flatDto.Description,
                Photo = photoBytes
            };
            RentHubContext.Instance.Flats.Add(flat);
            RentHubContext.Instance.SaveChanges();
            
            return Ok("Квартира успешно добавлена");
        }

        [HttpPut("ChangeFlatData{id}")]
        public ActionResult ChangeFlatData(int id, FlatDTO flatDto)
        {
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
            List<Flat> flats = RentHubContext.Instance.Flats.ToList();
            Flat? flat = flats.FirstOrDefault(fl => fl.FlatId == id);
            if(flat == null)
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
            RentHubContext.Instance.SaveChanges();

            return Ok("Данные квартиры успешно изменены");
        }

        [HttpDelete("DeleteFlat{id}")]
        public ActionResult DeleteFlat(int id)
        {
            List<Flat> flats = RentHubContext.Instance.Flats.ToList();
            Flat? flat = flats.FirstOrDefault(fl => fl.FlatId == id);
            if (flat == null)
            {
                return NotFound($"Квартира с ID {id} не найдена");
            }
            RentHubContext.Instance.Flats.Remove(flat);
            RentHubContext.Instance.SaveChanges();
            return Ok("Квартира успешно удалена");
        }
    }
}
