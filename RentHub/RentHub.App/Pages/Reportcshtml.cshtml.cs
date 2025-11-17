using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentHub.Core.Model;
using System.Net.Http.Json;

namespace RentHub.App.Pages
{
    public class ReportcshtmlModel : PageModel
    {
        private HttpClient _http = new HttpClient(); 

        public List<Flat> Flats { get; set; } = new();
        public List<ChartData> BookingsByMonth { get; set; } = new();
        public List<ChartData> OccupancyData { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? flatId = null)
        {
            try
            {
                string? token = Request.Cookies["jwt"];

                if (string.IsNullOrEmpty(token))
                    return RedirectToPage("/Welcome");

                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var data = await _http.GetFromJsonAsync<List<Flat>>("http://94.183.186.221:5000/Flats/user-flats");
                if (data != null)
                    Flats = data;

                var url = $"http://94.183.186.221:5000/Reservations/reservation-by-flat-id/{flatId}";

                var reservations = await _http.GetFromJsonAsync<List<Reservation>>(url);
                if (reservations != null)
                {
                    ProcessReservationData(reservations);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке квартир: {ex.Message}");
            }

            return Page();
        }
        private void ProcessReservationData(List<Reservation> reservations)
        {
            var bookingsByMonth = new Dictionary<string, int>();

            foreach (var r in reservations)
            {
                var key = $"{r.DateOfStartReservation:yyyy-MM}";
                bookingsByMonth[key] = bookingsByMonth.GetValueOrDefault(key, 0) + 1;
            }

            BookingsByMonth = bookingsByMonth.Select(kvp => new ChartData { Label = kvp.Key, Value = kvp.Value }).ToList();

            OccupancyData = BookingsByMonth.Select(item => new ChartData
            {
                Label = item.Label,
                Value = Math.Min(item.Value * 10, 100)
            }).ToList();
        }
    }

    public class ChartData
    {
        public string Label { get; set; } = "";
        public int Value { get; set; }
    }
}
