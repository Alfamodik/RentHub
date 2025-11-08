using Microsoft.AspNetCore.Mvc;
using RentHub.API.RequestModels.Avito;
using RentHub.API.ResponceModels.Avito;
using System.Net.Http.Headers;
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

        [HttpGet("get-bookings")]
        public async Task<IActionResult> GetBookings([FromQuery] AvitoBookingRequest avitoBookingRequest)
        {
            HttpClient _httpClient = new()
            {
                BaseAddress = new Uri("https://api.avito.ru/")
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", avitoBookingRequest.AccessToken);

            string dateStart = avitoBookingRequest.DateStart.ToString("yyyy-MM-dd");
            string dateEnd = avitoBookingRequest.DateEnd.ToString("yyyy-MM-dd");

            string skipError = avitoBookingRequest.SkipErrors.ToString().ToLower();
            string withUnpaid = avitoBookingRequest.WithUnpaid.ToString().ToLower();

            string url =
                @$"realty/v1/accounts/{avitoBookingRequest.UserId}/" +
                @$"items/{avitoBookingRequest.ItemId}/" +
                @$"bookings?skip_error={skipError}&" +
                @$"date_start={dateStart}&" +
                $@"date_end={dateEnd}&" +
                $@"with_unpaid={withUnpaid}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            AvitoBookingsResponse? bookingsResponse = JsonSerializer.Deserialize<AvitoBookingsResponse>(responseBody);

            return Ok(bookingsResponse?.Bookings);
        }
    }
}
