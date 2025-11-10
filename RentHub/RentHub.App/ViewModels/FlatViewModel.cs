namespace RentHub.App.ViewModels
{
    public class FlatViewModel
    {
        public int FlatId { get; set; }

        public int UserId { get; set; }

        public string Country { get; set; } = null!;

        public string City { get; set; } = null!;

        public string District { get; set; } = null!;

        public string HouseNumber { get; set; } = null!;

        public string ApartmentNumber { get; set; } = null!;

        public int RoomCount { get; set; }

        public decimal Size { get; set; }

        public int FloorNumber { get; set; }

        public int? FloorsNumber { get; set; }

        public string Description { get; set; } = null!;

        public byte[]? Photo { get; set; }
    }
}
