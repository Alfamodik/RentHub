namespace RentHub.Core.Model;

public partial class Reservation
{
    public int IdReservation { get; set; }

    public int IdAd { get; set; }

    public int IdRenter { get; set; }

    public DateOnly DateOfStartReservation { get; set; }

    public DateOnly DateOfEndReservation { get; set; }

    public decimal Summ { get; set; }

    public decimal Income { get; set; }

    public virtual Ad IdAdNavigation { get; set; } = null!;

    public virtual Renter IdRenterNavigation { get; set; } = null!;
}
