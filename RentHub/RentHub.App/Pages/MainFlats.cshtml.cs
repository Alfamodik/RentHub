
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

        [BindProperty]
        public FlatViewModel NewFlat { get; set; } = new FlatViewModel();

        [BindProperty]
        public IFormFile? PhotoUploadAdd { get; set; }

        [BindProperty]
        public int FlatIdToDelete { get; set; }

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

        public async Task<IActionResult> OnPostAddFlat()
        {
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            if (NewFlat == null)
            {
                NewFlat = new FlatViewModel();
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                MultipartFormDataContent content = new()
            {
                { new StringContent("21"), nameof(NewFlat.UserId) },
                { new StringContent(NewFlat.Country), nameof(NewFlat.Country) },
                { new StringContent(NewFlat.City), nameof(NewFlat.City) },
                { new StringContent(NewFlat.District), nameof(NewFlat.District) },
                { new StringContent(NewFlat.HouseNumber), nameof(NewFlat.HouseNumber) },
                { new StringContent(NewFlat.ApartmentNumber), nameof(NewFlat.ApartmentNumber) },
                { new StringContent(NewFlat.RoomCount.ToString()), nameof(NewFlat.RoomCount) },
                { new StringContent(NewFlat.Size.ToString()), nameof(NewFlat.Size) },
                { new StringContent(NewFlat.FloorNumber.ToString()), nameof(NewFlat.FloorNumber) },
                { new StringContent(NewFlat.FloorsNumber?.ToString() ?? ""), nameof(NewFlat.FloorsNumber) },
                { new StringContent(NewFlat.Description), nameof(NewFlat.Description) },
                
            };

                if (PhotoUploadAdd != null)
                {
                    using var ms = new MemoryStream();
                    await PhotoUploadAdd.CopyToAsync(ms);
                    ByteArrayContent fileContent = new(ms.ToArray());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(PhotoUploadAdd.ContentType);
                    content.Add(fileContent, "Photo", PhotoUploadAdd.FileName);
                }

                HttpResponseMessage response = await client.PostAsync("Flats/flat", content);
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
        public async Task<ActionResult> OnPostDeleteFlat()
        {
            try
            {
                string? token = Request.Cookies["jwt"];

                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Welcome");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"Flats/flat/{FlatIdToDelete}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["AddFlatMessage"] = " вартира успешно удалена!";
                    return RedirectToPage();
                }
                else
                {
                    string error = response.Content.ReadAsStringAsync().Result;
                    TempData["DeleteFlatError"] = response.StatusCode + ": " + error;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["DeleteFlatError"] = "ќшибка: " + ex.Message;
                return Page();
            }
        }
    }
}
