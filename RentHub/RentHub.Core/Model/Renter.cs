namespace RentHub.Core.Model;

public partial class Renter
{
    public int RenterId { get; set; }

    public string Name { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
