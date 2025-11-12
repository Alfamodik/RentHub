using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using RentHub.App.ViewModels;
using RentHub.Core.Model;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace RentHub.App.Pages
{
    public class MainFlatsModel : PageModel
    {

        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        public ObservableCollection<FlatViewModel>? Flats { get; set; }

        public async Task OnGet()
        {
            var token = Request.Cookies["jwt"];

            client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage response;
                response = await client.GetAsync("Flats/user-flats");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Flats = JsonSerializer.Deserialize<ObservableCollection<FlatViewModel>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    Flats = new ObservableCollection<FlatViewModel>();
                }
            }
            catch
            {
                Flats = new ObservableCollection<FlatViewModel>();
            }
        }

        public IActionResult OnPost(int flatId)
        {
            return RedirectToPage("/FlatDetails", new { id = flatId });
        }
    }
}
