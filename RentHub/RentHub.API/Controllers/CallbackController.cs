using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentHub.API.ResponceModels.Avito;
using RentHub.Core.Model;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string code)
        {
            Debug.Write(code);

            if (string.IsNullOrEmpty(code))
                return BadRequest("Код отсутствует.");

            string? claimedUserid = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            Debug.Write(claimedUserid);

            if (!int.TryParse(claimedUserid, out int userId))
                return Unauthorized();

            Debug.Write(userId);
            RentHubContext context = new();
            User? user = context.Users.FirstOrDefault(user => user.UserId == userId);

            if (user == null)
                return Unauthorized();

            HttpClient _httpClient = new()
            {
                BaseAddress = new Uri("https://api.avito.ru/")
            };

            Dictionary<string, string> data = new()
            {
                { "grant_type", "authorization_code" },
                { "client_id", "hqQSMZCzT_szv7cre5vG" },
                { "client_secret", "e8oWwQ9S7nsbmuBsr_MuALR8nqcRxtHtyDBrt-YN" },
                { "code", code },
                { "redirect_uri", "http://94.183.186.221:5168/Callback" }
            };

            Debug.Write(data);

            HttpResponseMessage response = await _httpClient.PostAsync("token", new FormUrlEncodedContent(data));
            string json = await response.Content.ReadAsStringAsync();

            Debug.Write(json);

            JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };

            AvitoAccessTokenResponse? accessTokenResponse = JsonSerializer.Deserialize<AvitoAccessTokenResponse>(json, jsonSerializerOptions);

            if (accessTokenResponse == null)
                return BadRequest("Ошибка десериализации ответа от Avito");

            Debug.Write(accessTokenResponse.AccessToken);
            Debug.Write(accessTokenResponse.RefreshToken);

            if (accessTokenResponse.AccessToken != null && accessTokenResponse.RefreshToken != null)
            {
                user.AvitoAccessToken = accessTokenResponse.AccessToken;
                user.AvitoRefreshToken = accessTokenResponse.RefreshToken;
                user.TokenExpiresAt = DateOnly.FromDateTime(DateTime.Now);
                context.SaveChanges();
                return Ok(accessTokenResponse);
            }

            return Ok(new { error = "Не удалось получить доступ" });
        }
    }
}
