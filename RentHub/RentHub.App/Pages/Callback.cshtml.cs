using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace RentHub.App.Pages
{
    public class CallbackModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Code { get; set; }

        private HttpClient _client = new()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        public async Task<IActionResult> OnGet()
        {
            HttpResponseMessage response = await _client.GetAsync($"Callback?code={Code}");
            return Redirect("Reservations");
        }
    }
}
