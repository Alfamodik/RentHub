using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentHub.App.ViewModels;
using RentHub.Core.Model;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace RentHub.App.Pages
{
    public class ReservationsModel : PageModel
    {
        public DateOnly CalendarStart;
        public int DaysCount;
        public List<DateOnly> Days;
        public List<FlatBookingsViewModel>? FlatBookingsViewModels;


        [BindProperty]
        public AdvertisimentViewModel newAdd { get; set; } = new AdvertisimentViewModel();

        private readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        private readonly RentHubContext _context = new();

        public void OnGet()
        {
            CalendarStart = DateOnly.FromDateTime(DateTime.Today.AddDays(-10));
            DaysCount = 40;

            Days = Enumerable.Range(0, DaysCount)
                .Select(CalendarStart.AddDays)
                .ToList();

            List<Flat> flats = _context.Flats
                .Include(flat => flat.Advertisements)
                .ThenInclude(advertisement => advertisement.Reservations)
                .ThenInclude(reservation => reservation.Renter)
                .Include(flat => flat.Advertisements)
                .ThenInclude(advertisement => advertisement.Platform)
                .OrderBy(f => f.FlatId)
                .ToList();

            FlatBookingsViewModels = flats.Select(flat =>
            {
                string photo;

                if (flat.Photo != null && flat.Photo.Length > 0)
                    photo = $"data:image/jpeg;base64,{Convert.ToBase64String(flat.Photo)}";
                else
                    photo = "/img/FlatPlaceholder.png";

                List<ReservationViewModel> reservations = new();

                foreach (Advertisement advertisement in flat.Advertisements)
                {
                    foreach (Reservation reservation in advertisement.Reservations)
                    {
                        ReservationViewModel? reservationViewModel = CreateReservationDisplay(reservation);

                        if (reservationViewModel != null)
                            reservations.Add(reservationViewModel);
                    }
                }

                return new FlatBookingsViewModel()
                {
                    FlatId = flat.FlatId,
                    Title = $"{flat.City}, {flat.HouseNumber}/{flat.ApartmentNumber} ({flat.RoomCount}ê)",
                    PhotoBase64 = photo,
                    Reservations = reservations,
                };
            }).ToList();
        }

        private ReservationViewModel? CreateReservationDisplay(Reservation reservation)
        {
            DateTime startDate = reservation.DateOfStartReservation.ToDateTime(TimeOnly.MinValue);
            DateTime endDate = reservation.DateOfEndReservation.ToDateTime(TimeOnly.MinValue);
            int startOffset = (startDate.Date - CalendarStart.ToDateTime(TimeOnly.MinValue).Date).Days;
            int endOffset = (endDate.Date - CalendarStart.ToDateTime(TimeOnly.MinValue).Date).Days;

            int clippedStart = Math.Max(0, startOffset);
            int clippedEnd = Math.Min(DaysCount - 1, endOffset);
            int length = clippedEnd - clippedStart + 1;

            if (length <= 0)
                return null;

            string colorHexCode;

            if (reservation.Advertisement.Platform.PlatformName == "Yandex")
                colorHexCode = "#4F5533";
            else if (reservation.Advertisement.Platform.PlatformName == "Sutochno")
                colorHexCode = "#612B2B";
            else if (reservation.Advertisement.Platform.PlatformName == "Avito")
                colorHexCode = "#345533";
            else
                colorHexCode = "#334155";

            return new ReservationViewModel
            {
                DateOfStartReservation = reservation.DateOfStartReservation,
                DateOfEndReservation = reservation.DateOfEndReservation,
                RenterName = $"{reservation.Renter.Name} {reservation.Renter.Patronymic}",
                PhoneNumber = reservation.Renter.PhoneNumber,
                ColorHexCode = colorHexCode
            };
        }

        public async Task<AdvertisimentViewModel?> GetAddOnFlatIdOtherPlatform()
        {
            var token = Request.Cookies["jwt"];

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage response;
                response = await client.GetAsync("Advertisements/advertisement-by-flat-id/platform-other/{}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    AdvertisimentViewModel? add = JsonSerializer.Deserialize<AdvertisimentViewModel>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return add;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<ActionResult> OnPostAddReservation()
        {
            var token = Request.Cookies["jwt"];
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            try
            {
                var jsonData = JsonSerializer.Serialize(newAdd);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("Renters/renter", content);
                if (response.IsSuccessStatusCode)
                {
                    OnGet();
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

    }
}
