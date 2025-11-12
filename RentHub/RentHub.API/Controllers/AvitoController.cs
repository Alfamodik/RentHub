using Microsoft.AspNetCore.Mvc;
using RentHub.API.RequestModels.Avito;
using RentHub.API.ResponceModels.Avito;
using RentHub.API.Services;
using RentHub.Core.Model;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AvitoController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] AvitoLoginRequest avitoLoginRequest)
        {
            HttpClient _httpClient = new()
            {
                BaseAddress = new Uri("https://api.avito.ru/")
            };

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = avitoLoginRequest.GrantType,
                ["client_id"] = avitoLoginRequest.ClientId,
                ["client_secret"] = avitoLoginRequest.ClientSecret
            };

            var content = new FormUrlEncodedContent(formData);

            HttpResponseMessage response = await _httpClient.PostAsync("token", content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            AvitoAccessTokenResponseExperement? accessTokenResponse = JsonSerializer.Deserialize<AvitoAccessTokenResponseExperement>(responseBody);

            return Ok(accessTokenResponse);
        }

        [HttpGet("get-user-id")]
        public async Task<IActionResult> GetUserId(string accessToken)
        {
            HttpClient _httpClient = new()
            {
                BaseAddress = new Uri("https://api.avito.ru/")
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _httpClient.GetAsync($"core/v1/accounts/self");
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            AvitoUserResponse? avitoUserResponse = JsonSerializer.Deserialize<AvitoUserResponse>(responseBody);

            return Ok(avitoUserResponse);
        }
    }
}
