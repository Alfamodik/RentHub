using System.Text.Json.Serialization;

namespace RentHub.API.RequestModels.Avito
{
    public class AvitoBookingsResponse
    {
        [JsonPropertyName("bookings")]
        public List<Booking> Bookings { get; set; }
    }

    public class Booking
    {
        [JsonPropertyName("avito_booking_id")]
        public long AvitoBookingId { get; set; }

        [JsonPropertyName("base_price")]
        public decimal BasePrice { get; set; }

        [JsonPropertyName("check_in")]
        public string CheckIn { get; set; }

        [JsonPropertyName("check_out")]
        public string CheckOut { get; set; }

        [JsonPropertyName("contact")]
        public BookingContact Contact { get; set; }

        [JsonPropertyName("guest_count")]
        public int GuestCount { get; set; }

        [JsonPropertyName("nights")]
        public int Nights { get; set; }

        [JsonPropertyName("safe_deposit")]
        public SafeDeposit SafeDeposit { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class BookingContact
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }
    }

    public class SafeDeposit
    {
        [JsonPropertyName("owner_amount")]
        public decimal OwnerAmount { get; set; }

        [JsonPropertyName("tax")]
        public decimal Tax { get; set; }

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }
    }
}
