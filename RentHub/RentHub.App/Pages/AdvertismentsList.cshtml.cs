using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentHub.API.ModelsDTO;
using RentHub.Core.Model;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RentHub.App.Pages
{
    public class AdvertismentsListModel : PageModel
    {
        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5188/")
        };

        public int FlatId { get; set; }

        public string? FlatAddressFromCookie { get; set; }

        public List<Advertisement>? Advertisements { get; set; }

        public List<PlacementPlatform>? Platforms { get; set; }

        [BindProperty]
        public string RentType { get; set; } = string.Empty;

        [BindProperty]
        public Advertisement? NewAdvertisement { get; set; }

        public async Task<IActionResult> OnGet(int flatId)
        {
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            FlatId = flatId;
            FlatAddressFromCookie = Request.Cookies["FlatAddress"];
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

        public async Task<IActionResult> OnPostAddAdvertisement(int FlatId, int PlatformId, string RentType, decimal PriceForPeriod, decimal IncomeForPeriod, string LinkToAdvertisement)
        {
            if (RentType != "Посуточно" && RentType != "Длительный период")
            {
                ModelState.AddModelError("RentType", "Недопустимый тип аренды.");
                return Page();
            }

            AdvertisementDTO advertisementDto = new AdvertisementDTO
            {
                FlatId = FlatId,
                PlatformId = PlatformId,
                RentType = RentType,
                PriceForPeriod = PriceForPeriod,
                IncomeForPeriod = IncomeForPeriod,
                LinkToAdvertisement = LinkToAdvertisement
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);

            var json = JsonSerializer.Serialize(advertisementDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("Advertisements/advertisement", content);
            return Redirect($"AdvertismentsList?flatId={FlatId}");
        }

        public async Task<IActionResult> OnPostDeleteAdvertisement(int AdvertisementIdToDelete, int FlatId)
        {
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.DeleteAsync($"Advertisements/advertisement/{AdvertisementIdToDelete}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Не удалось удалить объявление.");
            }

            return Redirect($"AdvertismentsList?flatId={FlatId}");
        }
        public ActionResult OnPostLogout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToPage("/Welcome");
        }
    }
}
