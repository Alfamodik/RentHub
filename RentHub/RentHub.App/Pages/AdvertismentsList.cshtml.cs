using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentHub.Core.Model;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RentHub.App.Pages
{
    public class AdvertismentsListModel : PageModel
    {
        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        public string? FlatIdFromCookie { get; set; }

        public string? FlatAddressFromCookie { get; set; }

        public List<Advertisement>? Advertisements { get; set; }

        public List<PlacementPlatform>? Platforms { get; set; }

        [BindProperty]
        public Advertisement? NewAdvertisement { get; set; }

        public async Task<IActionResult> OnGet(int? flatId = null)
        {
            FlatAddressFromCookie = Request.Cookies["SelectedFlatAddress"];

            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            response = await client.GetAsync($"Advertisements/flat/{flatId}");

            if (response.IsSuccessStatusCode)
            {
                string platformsJson = await response.Content.ReadAsStringAsync();
                Advertisements = JsonSerializer.Deserialize<List<Advertisement>>(platformsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage placementPlatformResponse;
            placementPlatformResponse = await client.GetAsync($"Platforms/platforms");

            if (placementPlatformResponse.IsSuccessStatusCode)
            {
                string platformsJson = await placementPlatformResponse.Content.ReadAsStringAsync();
                Platforms = JsonSerializer.Deserialize<List<PlacementPlatform>>(platformsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return Page();
        }
    }
}
