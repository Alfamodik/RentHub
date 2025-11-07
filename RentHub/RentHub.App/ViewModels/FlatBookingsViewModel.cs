namespace RentHub.App.ViewModels
{
    public class FlatBookingsViewModel
    {
        public int FlatId { get; set; }
        public string Title { get; set; } = "";
        public string PhotoBase64 { get; set; } = "";
        public List<ReservationViewModel> Reservations { get; set; } = new();
    }
}
