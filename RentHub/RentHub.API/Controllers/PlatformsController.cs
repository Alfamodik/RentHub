using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        [HttpGet("GetPlatforms")]
        public ActionResult<List<PlacementPlatform>> GetPlatforms()
        {
            List<PlacementPlatform> platformsList = RentHubContext.Instance.PlacementPlatforms.ToList();
            if (platformsList.IsNullOrEmpty())
            {
                return NotFound("Список платформ пуст");
            }
            return platformsList;
        }

        [HttpGet("GetPlatformById{id}")]
        public ActionResult<PlacementPlatform> GetPlatform(int id)
        {
            List<PlacementPlatform> platformsList = RentHubContext.Instance.PlacementPlatforms.ToList();
            PlacementPlatform? platform = platformsList.FirstOrDefault(p => p.PlatformId == id);

            if (platform == null)
            {
                return NotFound($"Платформа с ID {id} не найдена");
            }

            return Ok(platform);
        }

        [HttpPost("AddPlatform")]
        public ActionResult AddPlatform(PlatformDTO platformDTO)
        {
            PlacementPlatform placement = new PlacementPlatform
            {
                PlatformName = platformDTO.PlatformName
            };
            RentHubContext.Instance.PlacementPlatforms.Add(placement).Context.SaveChanges();
            return Ok("Платформа успешно добавлена");
        }

        [HttpPut("ChangePlatformData{id}")]
        public ActionResult ChangePlatformData(int id, PlatformDTO platformDTO)
        {
            List<PlacementPlatform> platformsList = RentHubContext.Instance.PlacementPlatforms.ToList();
            PlacementPlatform? platform = platformsList.FirstOrDefault(p => p.PlatformId == id);
            if (platform == null)
            {
                return NotFound($"Платформа с ID {id} не найдена");
            }
            platform.PlatformName = platformDTO.PlatformName;
            RentHubContext.Instance.SaveChanges();

            return Ok("Данные платформы успешно изменены");
        }

        [HttpDelete("DeletePlatform{id}")]
        public ActionResult DeletePlatform(int id)
        {
            List<PlacementPlatform> platformsList = RentHubContext.Instance.PlacementPlatforms.ToList();
            PlacementPlatform? platform = platformsList.FirstOrDefault(p => p.PlatformId == id);
            if (platform == null)
            {
                return NotFound($"Платформа с ID {id} не найдена");
            }
            RentHubContext.Instance.PlacementPlatforms.Remove(platform);
            RentHubContext.Instance.SaveChanges();
            return Ok("Платформа успешно удалена");
        }
    }
}
