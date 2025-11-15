namespace RentHub.API.ModelsDTO
{
    public class ReservationDTO
    {
        public int? ReservationId { get; set; }

        public int? AdvertisementId { get; set; }

        public int? RenterId { get; set; }

        public DateOnly? DateOfStartReservation { get; set; }

        public DateOnly? DateOfEndReservation { get; set; }

        public decimal? Summ { get; set; }

        public decimal? Income { get; set; }
    }
}
