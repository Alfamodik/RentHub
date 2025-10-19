using Microsoft.AspNetCore.Mvc;
using RentHub.API.RequestModels.Avito;
using System.Net.Http;

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Код отсутствует.");

            HttpClient _httpClient = new()
            {
                BaseAddress = new Uri("https://api.avito.ru/")
            };

            var data = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", "hqQSMZCzT_szv7cre5vG" },
                { "client_secret", "e8oWwQ9S7nsbmuBsr_MuALR8nqcRxtHtyDBrt-YN" },
                { "code", code },
                { "redirect_uri", "https://unshareable-flavia-swimmable.ngrok-free.dev/Callback" }
            };

            var response = await _httpClient.PostAsync("token/", new FormUrlEncodedContent(data));
            var content = await response.Content.ReadAsStringAsync();

            return Content(content, "application/json");
        }
    }
}
