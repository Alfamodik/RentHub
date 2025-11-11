namespace RentHub.Core.Model;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int AdvertisementId { get; set; }

    public int? RenterId { get; set; }

    public DateOnly DateOfStartReservation { get; set; }

    public DateOnly DateOfEndReservation { get; set; }

    public decimal Summ { get; set; }

    public decimal Income { get; set; }

    public virtual Advertisement Advertisement { get; set; } = null!;

    public virtual Renter Renter { get; set; } = null!;
}
