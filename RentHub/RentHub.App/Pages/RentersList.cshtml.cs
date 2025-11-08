using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using RentHub.App.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;

namespace RentHub.App.Pages
{
    public class RentersListModel : PageModel
    {
        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        public ObservableCollection<RenterViewModel>? Renters { get; set; }

        public async Task OnGet()
        {
            var token = Request.Cookies["jwt"];

            client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage response;
                response = await client.GetAsync("Renters/renters");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Renters = JsonSerializer.Deserialize<ObservableCollection<RenterViewModel>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    Renters = new ObservableCollection<RenterViewModel>();
                }
            }
            catch
            {
                Renters = new ObservableCollection<RenterViewModel>();
            }
        }
    }
}
