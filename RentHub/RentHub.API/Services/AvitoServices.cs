using Microsoft.AspNetCore.Mvc;
using RentHub.API.RequestModels.Avito;
using RentHub.API.ResponceModels.Avito;
using RentHub.Core.Model;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RentHub.API.Services
{
    public static class AvitoServices
    {
        private static readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("https://api.avito.ru/")
        };

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<List<Reservation>?> GetReservations(User user, string linkToAdvertisement)
        {
            if (user == null || user.AvitoRefreshToken == null)
                return null;

            if (DateTime.UtcNow >= user.TokenExpiresAt)
                await UpdateToken(user);

            if (DateTime.UtcNow >= user.TokenExpiresAt)
                return null;

            string path = new Uri(linkToAdvertisement).AbsolutePath;
            string lastSegment = path.Substring(path.LastIndexOf('/') + 1);
            Match match = Regex.Match(lastSegment, @"(\d+)$");

            if (!match.Success)
                return null;
            
            string itemId = match.Groups[1].Value;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.AvitoAccessToken);

            HttpResponseMessage accountResponse = await _httpClient.GetAsync($"core/v1/accounts/self");

            if (!accountResponse.IsSuccessStatusCode)
                return null;

            string accountResponseBody = await accountResponse.Content.ReadAsStringAsync();
            AvitoUserResponse? avitoUserResponse = JsonSerializer.Deserialize<AvitoUserResponse>(accountResponseBody, _jsonSerializerOptions);

            if (avitoUserResponse == null)
                return null;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.AvitoAccessToken);

            string dateStart = DateTime.Now.ToString("yyyy-MM-dd");
            string dateEnd = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            string skipError = true.ToString().ToLower();
            string withUnpaid = false.ToString().ToLower();

            string url =
                @$"realty/v1/accounts/{avitoUserResponse.Id}/" +
                @$"items/{itemId}/" +
                @$"bookings?skip_error={skipError}&" +
                @$"date_start={dateStart}&" +
                $@"date_end={dateEnd}&" +
                $@"with_unpaid={withUnpaid}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!accountResponse.IsSuccessStatusCode)
                return null;

            string responseBody = await response.Content.ReadAsStringAsync();
            AvitoBookingsResponse? bookingsResponse = JsonSerializer.Deserialize<AvitoBookingsResponse>(responseBody, _jsonSerializerOptions);

            if (bookingsResponse == null)
                return null;

            List<Reservation> reservations = new();
            //RentHubContext context = new();

            foreach (Booking booking in bookingsResponse.Bookings)
            {
                /*Renter? renter = context.Renters.FirstOrDefault(renter => renter.Name == booking.Contact.Name);
                renter ??= new()
                {
                    Name = booking.Contact.Name,
                    PhoneNumber = booking.Contact.Phone
                };*/

                Reservation reservation = new()
                {
                    DateOfStartReservation = booking.CheckIn,
                    DateOfEndReservation = booking.CheckOut,
                };

                reservations.Add(reservation);
            }

            return reservations;
        }

        private static async Task UpdateToken(User user)
        {
            Dictionary<string, string> data = new()
            {
                { "client_id", "hqQSMZCzT_szv7cre5vG" },
                { "client_secret", "e8oWwQ9S7nsbmuBsr_MuALR8nqcRxtHtyDBrt-YN" },
                { "grant_type", "refresh_token" },
                { "refresh_token", $"{user.AvitoRefreshToken}" },
            };

            HttpResponseMessage accessTokenResponse = await _httpClient.PostAsync("token", new FormUrlEncodedContent(data));

            if (!accessTokenResponse.IsSuccessStatusCode)
                return;

            string json = await accessTokenResponse.Content.ReadAsStringAsync();
            
            AvitoAccessTokenResponse? avitoAccessTokenResponse = JsonSerializer.Deserialize<AvitoAccessTokenResponse>(json, _jsonSerializerOptions);

            if (avitoAccessTokenResponse == null)
                return;

            user.AvitoAccessToken = avitoAccessTokenResponse.AccessToken;
            user.AvitoRefreshToken = avitoAccessTokenResponse.RefreshToken;
            user.TokenExpiresAt = DateTime.Now.AddDays(1);
        }
    }
}
