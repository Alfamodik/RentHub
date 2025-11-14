using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using RentHub.App.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Text;
using System.Net.Http.Headers;

namespace RentHub.App.Pages
{
    public class RentersListModel : PageModel
    {
        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        public ObservableCollection<RenterViewModel>? Renters { get; set; }

        [BindProperty]
        public RenterViewModel NewRenter { get; set; } = new RenterViewModel();

        [BindProperty]
        public int RenterIdToDelete { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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

            return Page();
        }
        public async Task<ActionResult> OnPostAddRenter()
        {
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            if (NewRenter == null)
            {
                NewRenter = new RenterViewModel();
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var jsonData = JsonSerializer.Serialize(NewRenter);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("Renters/renter", content);
                if (response.IsSuccessStatusCode)
                {
                    await OnGet();
                    return RedirectToPage();
                }
                else
                {
                    return Page();
                }
            }
            catch
            {
                return Page();
            }
        }
        public async Task<ActionResult> OnPostDeleteRenter()
        {
            try
            {
                string? token = Request.Cookies["jwt"];

                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Welcome");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"Renters/renter/{RenterIdToDelete}");

                if (response.IsSuccessStatusCode)
                {
                    await OnGet();
                    return RedirectToPage();
                }
                else
                {
                    return Page();
                }
            }
            catch
            {
                return Page();
            }
        }
        public ActionResult OnPostLogout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToPage("/Welcome");
        }
    }
}
