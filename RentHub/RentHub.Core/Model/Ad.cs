using System;
using System.Collections.Generic;

namespace RentHub.Core.Model;

public partial class Ad
{
    public int IdAd { get; set; }

    public int IdFlat { get; set; }

    public int IdPlatform { get; set; }

    public string RentType { get; set; } = null!;

    public decimal PriceForPeriod { get; set; }

    public decimal IncomeForPeriod { get; set; }

    public string LinkToAd { get; set; } = null!;

    public virtual Flat IdFlatNavigation { get; set; } = null!;

    public virtual PlacementPlatform IdPlatformNavigation { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
