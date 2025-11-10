using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        [Authorize]
        [HttpGet("advertisements")]
        public ActionResult<List<Advertisement>> GetAdvertisements()
        {
            using RentHubContext context = new();
            List<Advertisement> AdvertisementsList = context.Advertisements.ToList();
            if (AdvertisementsList.IsNullOrEmpty())
            {
                return NotFound("Список объявлений пуст");
            }
            return AdvertisementsList;
        }

        [Authorize]
        [HttpGet("advertisement-by-id/{id}")]
        public ActionResult<Advertisement> GetAdvertisement(int id)
        {
            using RentHubContext context = new();
            Advertisement? advertisement = context.Advertisements.FirstOrDefault(r => r.AdvertisementId == id);

            if (advertisement == null)
            {
                return NotFound($"Объявление с ID {id} не найдено");
            }

            return Ok(advertisement);
        }

        [Authorize]
        [HttpGet("flat/{id}")]
        public ActionResult<IEnumerable<Advertisement>> GetFlatAdvertisements(int id)
        {
            using RentHubContext context = new();
            List<Advertisement>? advertisements = context.Advertisements
                .Where(advertisement => advertisement.FlatId == id)
                .Include(advertisement => advertisement.Platform)
                .ToList();

            return Ok(advertisements);
        }

        [HttpGet("advertisement-by-flat-id/platform-other/{id}")]
        public ActionResult<Advertisement> GetAdvertisementOfFlat(int id)
        {
            using RentHubContext context = new();
            Advertisement? advertisement = context.Advertisements.FirstOrDefault(r => r.FlatId == id && r.PlatformId == 3);

            if (advertisement == null)
            {
                return NotFound($"Объявление квартиры с прочей платформой с ID {id} не найдено");
            }

            return Ok(advertisement);
        }

        [Authorize]
        [HttpPost("advertisement")]
        public ActionResult AddAdvertisement(AdvertisementDTO advertisementdto)
        {
            using RentHubContext context = new();
            Advertisement advertisement = new Advertisement
            {
                FlatId = advertisementdto.FlatId,
                PlatformId = advertisementdto.PlatformId,
                RentType = advertisementdto.RentType,
                PriceForPeriod = advertisementdto.PriceForPeriod,
                IncomeForPeriod = advertisementdto.IncomeForPeriod,
                LinkToAdvertisement = advertisementdto.LinkToAdvertisement
            };
            context.Advertisements.Add(advertisement).Context.SaveChanges();
            return Ok("Объявление успешно добавлено");
        }

        [Authorize]
        [HttpPut("advertisement-data/{id}")]
        public ActionResult ChangeAdvertisementData(int id, AdvertisementDTO advertisementdto)
        {
            using RentHubContext context = new();
            Advertisement? advertisement = context.Advertisements.FirstOrDefault(item => item.AdvertisementId == id);
            if (advertisement == null)
            {
                return NotFound($"Объявление с ID {id} не найдено");
            }
            advertisement.FlatId = advertisementdto.FlatId;
            advertisement.PlatformId = advertisementdto.PlatformId;
            advertisement.RentType = advertisementdto.RentType;
            advertisement.PriceForPeriod = advertisementdto.PriceForPeriod;
            advertisement.IncomeForPeriod = advertisementdto.IncomeForPeriod;
            advertisement.LinkToAdvertisement = advertisementdto.LinkToAdvertisement;
            context.SaveChanges();

            return Ok("Данные объявления успешно изменены");
        }

        [Authorize]
        [HttpDelete("advertisement/{id}")]
        public ActionResult DeleteAdvertisement(int id)
        {
            using RentHubContext context = new();
            Advertisement? advertisement = context.Advertisements
                .Include(item => item.Reservations)
                .FirstOrDefault(r => r.AdvertisementId == id);
            
            if (advertisement == null)
            {
                return NotFound($"Объявление с ID {id} не найдено");
            }

            context.Reservations.RemoveRange(advertisement.Reservations);
            context.Advertisements.Remove(advertisement);
            context.SaveChanges();

            return Ok("Объявление успешно удалено");
        }
    }
}
