using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Net.Http.Headers;
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
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.GetAsync($"Callback?code={Code}");
            Debug.Write(await response.Content.ReadAsStringAsync());
            return Redirect("Reservations");
        }
    }
}
