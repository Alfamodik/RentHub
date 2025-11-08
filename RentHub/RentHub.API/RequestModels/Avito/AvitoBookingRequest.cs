using System.ComponentModel.DataAnnotations;

namespace RentHub.API.RequestModels.Avito
{
    public class AvitoBookingRequest
    {
        public string AccessToken { get; set; } = null!;

        public ulong UserId { get; set; }

        public ulong ItemId { get; set; }

        public bool SkipErrors { get; set; }

        [DataType(DataType.Date)]
        public DateOnly DateStart { get; set; }

        [DataType(DataType.Date)]
        public DateOnly DateEnd { get; set; }

        public bool WithUnpaid { get; set; }
    }
}
