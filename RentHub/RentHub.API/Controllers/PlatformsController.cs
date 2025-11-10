using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        [Authorize]
        [HttpGet("platforms")]
        public ActionResult<List<PlacementPlatform>> GetPlatforms()
        {
            using RentHubContext context = new();
            List<PlacementPlatform> platformsList = context.PlacementPlatforms.ToList();
            if (platformsList.IsNullOrEmpty())
            {
                return NotFound("Список платформ пуст");
            }
            return platformsList;
        }

        [Authorize]
        [HttpGet("platform-by-id/{id}")]
        public ActionResult<PlacementPlatform> GetPlatform(int id)
        {
            using RentHubContext context = new();
            PlacementPlatform? platform = context.PlacementPlatforms.FirstOrDefault(p => p.PlatformId == id);

            if (platform == null)
            {
                return NotFound($"Платформа с ID {id} не найдена");
            }

            return Ok(platform);
        }

        [Authorize]
        [HttpPost("platform")]
        public ActionResult AddPlatform(PlatformDTO platformDTO)
        {
            using RentHubContext context = new();
            PlacementPlatform placement = new PlacementPlatform
            {
                PlatformName = platformDTO.PlatformName
            };
            context.PlacementPlatforms.Add(placement).Context.SaveChanges();
            return Ok("Платформа успешно добавлена");
        }

        [Authorize]
        [HttpPut("platform-data/{id}")]
        public ActionResult ChangePlatformData(int id, PlatformDTO platformDTO)
        {
            using RentHubContext context = new();
            PlacementPlatform? platform = context.PlacementPlatforms.FirstOrDefault(p => p.PlatformId == id);

            if (platform == null)
            {
                return NotFound($"Платформа с ID {id} не найдена");
            }
            platform.PlatformName = platformDTO.PlatformName;
            context.SaveChanges();

            return Ok("Данные платформы успешно изменены");
        }

        [Authorize]
        [HttpDelete("platform/{id}")]
        public ActionResult DeletePlatform(int id)
        {
            using RentHubContext context = new();
            PlacementPlatform? platform = context.PlacementPlatforms.FirstOrDefault(p => p.PlatformId == id);

            if (platform == null)
            {
                return NotFound($"Платформа с ID {id} не найдена");
            }
            context.PlacementPlatforms.Remove(platform);
            context.SaveChanges();
            return Ok("Платформа успешно удалена");
        }
    }
}
