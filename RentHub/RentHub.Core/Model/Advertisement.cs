using System.Text.Json.Serialization;

namespace RentHub.Core.Model;

public partial class Advertisement
{
    public int AdvertisementId { get; set; }

    public int FlatId { get; set; }

    public int PlatformId { get; set; }

    public string RentType { get; set; } = null!;

    public decimal PriceForPeriod { get; set; }

    public decimal IncomeForPeriod { get; set; }

    public string LinkToAdvertisement { get; set; } = string.Empty;

    public virtual Flat Flat { get; set; } = null!;

    public virtual PlacementPlatform Platform { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
