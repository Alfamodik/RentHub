using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentHub.Core.Model;
using System.Net.Http.Json;

namespace RentHub.App.Pages
{
    public class ReportcshtmlModel : PageModel
    {
        private HttpClient _http = new HttpClient(); // статический клиент, без фабрики

        public List<Flat> Flats { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                string? token = Request.Cookies["jwt"];

                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Welcome");

                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var data = await _http.GetFromJsonAsync<List<Flat>>("http://94.183.186.221:5000/Flats/flats");
                if (data != null)
                    Flats = data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке квартир: {ex.Message}");
            }

            return Page();
        }
    }
}
