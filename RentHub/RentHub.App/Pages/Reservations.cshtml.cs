using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentHub.App.ResponseModels;
using RentHub.App.ViewModels;
using RentHub.Core.Model;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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

        private List<Flat>? _flats;
        public ObservableCollection<RenterViewModel>? Renters { get; set; } = new ObservableCollection<RenterViewModel>();
        public ObservableCollection<FlatViewModel>? Flats { get; set; } = new ObservableCollection<FlatViewModel>();

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
            await Getrenters();
            await GetFlats();

            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            CalendarStart = DateOnly.FromDateTime(DateTime.Today.AddDays(-DateTime.Today.Day + 1));
            DateOnly firstDayOfNextMonth = CalendarStart.AddMonths(2);
            DateOnly lastDayOfCurrentMonth = firstDayOfNextMonth.AddDays(-1);
            DaysCount = lastDayOfCurrentMonth.DayNumber - CalendarStart.DayNumber + 1;

            Days = Enumerable.Range(0, DaysCount)
                .Select(CalendarStart.AddDays)
                .ToList();
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _client.GetAsync($"Flats/user-flats");

            if (response.IsSuccessStatusCode)
            {
                string platformsJson = await response.Content.ReadAsStringAsync();
                _flats = JsonSerializer.Deserialize<List<Flat>>(platformsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            _flats ??= new();
            FlatBookingsViewModels = _flats.Select(flat =>
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
                        ReservationViewModel? reservationViewModel = CreateReservationDisplay(reservation, advertisement);

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

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage updateResponse = await _client.GetAsync($"Reservations/update");
            string responseContent = await updateResponse.Content.ReadAsStringAsync();
            return Page();
        }

        private async Task Getrenters()
        {
            var token = Request.Cookies["jwt"];

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage response;
                response = await _client.GetAsync("Renters/renters");

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
        private async Task GetFlats()
        {
            var token = Request.Cookies["jwt"];

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"Flats/user-flats");

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


        public async Task<IActionResult> OnPostRefreshReservations()
        {
            string? token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Welcome");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.GetAsync($"Flats/user-flats");

            if (response.IsSuccessStatusCode)
            {
                string platformsJson = await response.Content.ReadAsStringAsync();
                _flats = JsonSerializer.Deserialize<List<Flat>>(platformsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            if (_flats?.Any(flat => flat.Advertisements.Any(advertisement => advertisement.Platform.PlatformName == "Avito")) == true)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage accessResponse = await _client.GetAsync($"Authentication/has-avito-access");

                if (response.IsSuccessStatusCode)
                {
                    string platformsJson = await accessResponse.Content.ReadAsStringAsync();
                    HasAvitoAsseccResponse? hasAvitoAsseccResponse = JsonSerializer.Deserialize<HasAvitoAsseccResponse>(platformsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (hasAvitoAsseccResponse?.HasAccess == false)
                        return Redirect("https://www.avito.ru/oauth?response_type=code&client_id=hqQSMZCzT_szv7cre5vG&scope=short_term_rent:read,user:read");
                }
            }

            return Page();
        }

        private ReservationViewModel? CreateReservationDisplay(Reservation reservation, Advertisement advertisement)
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

            if (advertisement.Platform.PlatformName == "Yandex")
                colorHexCode = "#4F5533";
            else if (advertisement.Platform.PlatformName == "Sutochno")
                colorHexCode = "#612B2B";
            else if (advertisement.Platform.PlatformName == "Avito")
                colorHexCode = "#345533";
            else
                colorHexCode = "#334155";

            return new ReservationViewModel
            {
                Id = reservation.ReservationId,
                DateOfStartReservation = reservation.DateOfStartReservation,
                DateOfEndReservation = reservation.DateOfEndReservation,
                RenterName = $"{reservation.Renter?.Name} {reservation.Renter?.Patronymic}",
                PhoneNumber = $"{reservation.Renter?.PhoneNumber}",
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
                    string error = await response.Content.ReadAsStringAsync();
                    TempData["ReservationMessage"] = response.StatusCode + ": " + error;
                    return null;
                }
            }
            catch (Exception ex)
            {
                TempData["ReservationMessage"] = "Ошибка: " + ex.Message;
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
                    TempData["ReservationMessage"] = response.StatusCode + ": " + error;
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
        public ActionResult OnPostLogout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToPage("/Welcome");
        }
    }
}
