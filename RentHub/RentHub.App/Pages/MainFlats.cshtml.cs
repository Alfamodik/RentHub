
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using RentHub.App.ViewModels;
using RentHub.Core.Model;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;

namespace RentHub.App.Pages
{
    public class MainFlatsModel : PageModel
    {

        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };
        private readonly HttpClient clientTESTAPI = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5188/")
        };

        [BindProperty]
        public FlatViewModel NewFlat { get; set; } = new FlatViewModel();

        [BindProperty]
        public IFormFile? PhotoUpload { get; set; }

        public ObservableCollection<FlatViewModel>? Flats { get; set; }

        public async Task<ActionResult> OnGet()
        {

            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

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
                    return Page();
                }
                else
                {
                    Flats = new ObservableCollection<FlatViewModel>();
                    return Page();
                }
            }
            catch
            {
                Flats = new ObservableCollection<FlatViewModel>();
                return Page();
            }
        }

        public ActionResult OnPostLogout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToPage("/Welcome");
        }

        public ActionResult OnPostSaveChange(int flatId)
        {
            return RedirectToPage("/FlatDetails", new { id = flatId });
        }

        public async Task<ActionResult> OnPostAddFlat()
        {
            //string? token = Request.Cookies["jwt"];
            string? token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjIxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoibmV3bGVnYTIwMjFAZ21haWwuY29tIiwiZXhwIjoxNzYzMDMzNTk3LCJpc3MiOiJSZW50SHViIiwiYXVkIjoiUmVudEh1YlVzZXJzIn0.R7VHVVc-kAhRcacQ9KEKRIGAaFHB4vd3NId2LrG6jQs";

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            if (NewFlat == null)
            {
                NewFlat = new FlatViewModel();
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var jsonData = JsonSerializer.Serialize(NewFlat);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await clientTESTAPI.PostAsync("Flats/flat", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["AddFlatMessage"] = " вартира успешно добавлена!";
                    return RedirectToPage();
                }
                else
                {
                    string error = response.Content.ReadAsStringAsync().Result;
                    TempData["AddFlatError"] = response.StatusCode + ": " + error;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["AddFlatError"] = "ќшибка: " + ex.Message;
                return Page();
            }
        }
    }
}
