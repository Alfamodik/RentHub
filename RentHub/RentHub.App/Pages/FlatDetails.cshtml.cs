using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentHub.App.ViewModels;
using RentHub.Core.Model;
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

        [BindProperty]
        public FlatViewModel? Flat { get; set; }

        [BindProperty]
        public IFormFile? PhotoUpload { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync($"Flats/flat-by-id/{Id}");

            if (!response.IsSuccessStatusCode)
                return Page();

            string json = await response.Content.ReadAsStringAsync();

            Flat flat = JsonSerializer.Deserialize<Flat>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            Flat = new FlatViewModel
            {
                FlatId = flat.FlatId,
                Country = flat.Country,
                City = flat.City,
                District = flat.District,
                HouseNumber = flat.HouseNumber,
                ApartmentNumber = flat.ApartmentNumber,
                RoomCount = flat.RoomCount,
                Size = flat.Size,
                FloorNumber = flat.FloorNumber,
                FloorsNumber = flat.FloorsNumber,
                Description = flat.Description,
                Photo = flat.Photo
            };

            Response.Cookies.Append("FlatAddress", Flat.FullAddress, new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            MultipartFormDataContent content = new()
            {
                { new StringContent(Flat.Country), nameof(Flat.Country) },
                { new StringContent(Flat.City), nameof(Flat.City) },
                { new StringContent(Flat.District), nameof(Flat.District) },
                { new StringContent(Flat.HouseNumber), nameof(Flat.HouseNumber) },
                { new StringContent(Flat.ApartmentNumber), nameof(Flat.ApartmentNumber) },
                { new StringContent(Flat.RoomCount.ToString()), nameof(Flat.RoomCount) },
                { new StringContent(Flat.Size.ToString()), nameof(Flat.Size) },
                { new StringContent(Flat.FloorNumber.ToString()), nameof(Flat.FloorNumber) },
                { new StringContent(Flat.FloorsNumber?.ToString() ?? ""), nameof(Flat.FloorsNumber) },
                { new StringContent(Flat.Description), nameof(Flat.Description) }
            };

            if (PhotoUpload != null)
            {
                using var ms = new MemoryStream();
                await PhotoUpload.CopyToAsync(ms);
                ByteArrayContent fileContent = new(ms.ToArray());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(PhotoUpload.ContentType);
                content.Add(fileContent, "Photo", PhotoUpload.FileName);
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);

            var response = await client.PutAsync($"Flats/flat-data/{Flat.FlatId}", content);
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/MainFlats");
            }
            
            ModelState.AddModelError(string.Empty, "Ошибка при сохранении данных квартиры");
            return Page();
        }
        public ActionResult OnPostLogout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToPage("/Welcome");
        }
    }
}
