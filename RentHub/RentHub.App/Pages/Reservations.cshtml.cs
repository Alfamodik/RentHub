using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentHub.App.ViewModels;
using RentHub.Core.Model;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentHub.App.Pages
{
    public class ReservationsModel : PageModel
    {
        public DateOnly CalendarStart;
        public int DaysCount;
        public List<DateOnly>? Days = new List<DateOnly>();
        public List<FlatBookingsViewModel>? FlatBookingsViewModels;


        [BindProperty]
        public ReservationFullViewModel newReservation { get; set; } = new ReservationFullViewModel();

        private readonly HttpClient _client = new()
        {
            BaseAddress = new Uri("http://94.183.186.221:5000/")
        };

        [BindProperty]
        public int FlatID { get; set; }
        [BindProperty]
        public int ReservationID { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            CalendarStart = DateOnly.FromDateTime(DateTime.Today.AddDays(-10));
            DaysCount = 40;

            Days = Enumerable.Range(0, DaysCount)
                .Select(CalendarStart.AddDays)
                .ToList();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            response = await _client.GetAsync($"Flats/user-flats");

            List<Flat>? flats = null;

            if (response.IsSuccessStatusCode)
            {
                string platformsJson = await response.Content.ReadAsStringAsync();
                flats = JsonSerializer.Deserialize<List<Flat>>(platformsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            flats ??= new();
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
                    Title = $"{flat.City}, {flat.HouseNumber}/{flat.ApartmentNumber} ({flat.RoomCount}к)",
                    PhotoBase64 = photo,
                    Reservations = reservations,
                };
            }).ToList();

            return Page();
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
                Id = reservation.ReservationId,
                DateOfStartReservation = reservation.DateOfStartReservation,
                DateOfEndReservation = reservation.DateOfEndReservation,
                RenterName = $"{reservation.Renter.Name} {reservation.Renter.Patronymic}",
                PhoneNumber = reservation.Renter.PhoneNumber,
                ColorHexCode = colorHexCode
            };
        }

        public async Task<AdvertisimentViewModel?> GetFlatIdOtherPlatform()
        {
            var token = Request.Cookies["jwt"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage response;
                response = await _client.GetAsync($"Advertisements/advertisement-by-flat-id/platform-other/{FlatID}");

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
            AdvertisimentViewModel? add = await GetFlatIdOtherPlatform();
            if (add == null)
            {
                return Page();
            }

            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                newReservation.AdvertisementId = add.AdvertisementId;
                newReservation.Summ = Convert.ToDecimal((newReservation.DateOfEndReservation.DayOfYear - newReservation.DateOfStartReservation.DayOfYear) * add.PriceForPeriod);
                newReservation.Income = Convert.ToDecimal((newReservation.DateOfEndReservation.DayOfYear - newReservation.DateOfStartReservation.DayOfYear) * add.PriceForPeriod);
                var jsonData = JsonSerializer.Serialize(newReservation);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync("Reservations/reservation", content);
                
                if (response.IsSuccessStatusCode)
                {
                    await OnGet();
                    TempData["ReservationMessage"] = "Бронирование успешно добавлено!";
                    return RedirectToPage();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    TempData["ReservationMessage"] = "" + error;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ReservationMessage"] = "Ошибка: " + ex.Message;
                return Page();
            }
        }

        public async Task<ActionResult> OnPostDeleteReservationAsync(int reservationIdToDelete)
        {
            try
            {
                string? token = Request.Cookies["jwt"];

                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Welcome");

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _client.DeleteAsync($"Reservations/reservation/{reservationIdToDelete}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    TempData["ReservationMessage"] = "Ошибка при удалении бронирования";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ReservationMessage"] = "Ошибка: " + ex.Message;
                return Page();
            }
        }
    }
}
