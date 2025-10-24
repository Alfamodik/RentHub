using RentHub.Core.Model;
using System.Text.Json.Serialization;

namespace RentHub.API.ModelsDTO
{
    public class FlatDTO
    {
        public int FlatId { get; set; }

        public string Country { get; set; } = null!;

        public string City { get; set; } = null!;

        public string District { get; set; } = null!;

        public string HouseNumber { get; set; } = null!;

        public int RoomCount { get; set; }

        public decimal Size { get; set; }

        public int FloorNumber { get; set; }

        public int? FloorsNumber { get; set; }

        public string Description { get; set; } = null!;

        public IFormFile? Photo { get; set; }

    }
}
