using System;
using System.Collections.Generic;

namespace RentHub.Core.Model;

public partial class Renter
{
    public int RenterId { get; set; }

    public string Name { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Patronymic { get; set; }

    /// <summary>
    /// +7 900 304 93 12
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
