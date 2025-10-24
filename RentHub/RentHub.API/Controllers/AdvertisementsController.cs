using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        [HttpGet("GetAdvertisements")]
        public ActionResult<List<Advertisement>> GetAdvertisements()
        {
            List<Advertisement> AdvertisementsList = RentHubContext.Instance.Advertisements.ToList();
            if (AdvertisementsList.IsNullOrEmpty())
            {
                return NotFound("Список объявлений пуст");
            }
            return AdvertisementsList;
        }

        [HttpGet("GetAdvertisementById{id}")]
        public ActionResult<Advertisement> GetAdvertisement(int id)
        {
            List<Advertisement> AdvertisementsList = RentHubContext.Instance.Advertisements.ToList();
            Advertisement? advertisement = AdvertisementsList.FirstOrDefault(r => r.AdvertisementId == id);

            if (advertisement == null)
            {
                return NotFound($"Объявление с ID {id} не найдено");
            }

            return Ok(advertisement);
        }

        [HttpPost("AddAdvertisement")]
        public ActionResult AddAdvertisement(AdvertisementDTO advertisementdto)
        {
            Advertisement advertisement = new Advertisement
            {
                FlatId = advertisementdto.FlatId,
                PlatformId = advertisementdto.PlatformId,
                RentType = advertisementdto.RentType,
                PriceForPeriod = advertisementdto.PriceForPeriod,
                IncomeForPeriod = advertisementdto.IncomeForPeriod,
                LinkToAdvertisement = advertisementdto.LinkToAdvertisement
            };
            RentHubContext.Instance.Advertisements.Add(advertisement).Context.SaveChanges();
            return Ok("Объявление успешно добавлено");
        }

        [HttpPut("ChangeAdvertisementData{id}")]
        public ActionResult ChangeAdvertisementData(int id, AdvertisementDTO advertisementdto)
        {
            List<Advertisement> AdvertisementsList = RentHubContext.Instance.Advertisements.ToList();
            Advertisement? advertisement = AdvertisementsList.FirstOrDefault(r => r.AdvertisementId == id);
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
            RentHubContext.Instance.SaveChanges();

            return Ok("Данные объявления успешно изменены");
        }

        [HttpDelete("DeleteAdvertisement{id}")]
        public ActionResult DeleteAdvertisement(int id)
        {
            List<Advertisement> AdvertisementsList = RentHubContext.Instance.Advertisements.ToList();
            Advertisement? advertisement = AdvertisementsList.FirstOrDefault(r => r.AdvertisementId == id);
            if (advertisement == null)
            {
                return NotFound($"Объявление с ID {id} не найдено");
            }
            RentHubContext.Instance.Advertisements.Remove(advertisement).Context.SaveChanges();
            return Ok("Объявление успешно удалено");
        }
    }
}
