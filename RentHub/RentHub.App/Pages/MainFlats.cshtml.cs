using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using RentHub.Core.Model;
using System.Text.Json;

namespace RentHub.App.Pages
{
    public class MainFlatsModel : PageModel
    {

        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool emailExists { get; set; }

        public List<Flat>? Flats { get; set; } = new();
        public void OnGet()
        {
            if (TempData.TryGetValue("Email", out var e))
                Email = e?.ToString() ?? string.Empty;

            if (TempData.TryGetValue("exists", out var ex))
                emailExists = string.Equals(ex?.ToString(), "true", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ActionResult> GetFlatsAsync()
        {
            try
            {
                HttpResponseMessage response;
                response = await client.GetAsync("Flats/flats");
                string body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Message"] = $"Ошибка API: {response.StatusCode}. {body}";
                    return BadRequest();
                }
                var json = response.Content.ReadAsStringAsync().Result;
                List<Flat>? flats = JsonSerializer.Deserialize<List<Flat>>(json);
                if (flats.IsNullOrEmpty())
                {
                    TempData["Message"] = "Список квартир пустой";
                    return NotFound();
                }
                else
                {
                    Flats = flats;
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Ошибка: " + ex.Message;
                return NotFound();
            }
        }
    }
}
