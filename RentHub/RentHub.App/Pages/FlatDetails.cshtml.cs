using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentHub.App.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RentHub.App.Pages
{
    public class FlatDetailsModel : PageModel
    {
        private readonly HttpClient client = new()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public FlatViewModel? Flat { get; set; }

        public async Task OnGet()
        {
            string? token = Request.Cookies["jwt"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync($"Flats/flat-by-id/{Id}");

            if (!response.IsSuccessStatusCode)
                return;

            string json = await response.Content.ReadAsStringAsync();

            Flat = JsonSerializer.Deserialize<FlatViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
