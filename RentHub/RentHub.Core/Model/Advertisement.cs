namespace RentHub.Core.Model;

public partial class Advertisement
{
    public int AdvertisementId { get; set; }

    public int FlatId { get; set; }

    public int PlatformId { get; set; }

    public string RentType { get; set; } = null!;

    public double PriceForPeriod { get; set; }

    public double IncomeForPeriod { get; set; }

    public string LinkToAdvertisement { get; set; } = null!;

    public virtual Flat Flat { get; set; } = null!;

    public virtual PlacementPlatform Platform { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
