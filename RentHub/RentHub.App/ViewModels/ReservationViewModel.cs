namespace RentHub.App.ViewModels
{
    public class ReservationViewModel
    {
        public DateOnly DateOfStartReservation { get; set; }
        public DateOnly DateOfEndReservation { get; set; }
        public string RenterName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string ColorHexCode { get; set; } = "";
    }
}
