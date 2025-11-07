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
            Advertisement? advertisement = context.Advertisements.FirstOrDefault(r => r.AdvertisementId == id);
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
            Advertisement? advertisement = context.Advertisements.FirstOrDefault(r => r.AdvertisementId == id);
            if (advertisement == null)
            {
                return NotFound($"Объявление с ID {id} не найдено");
            }
            context.Advertisements.Remove(advertisement).Context.SaveChanges();
            return Ok("Объявление успешно удалено");
        }
    }
}
