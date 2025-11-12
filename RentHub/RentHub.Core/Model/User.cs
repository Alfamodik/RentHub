namespace RentHub.Core.Model;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? AvitoAccessToken { get; set; }

    public string? AvitoRefreshToken { get; set; }

    public DateOnly? TokenExpiresAt { get; set; }

    public virtual ICollection<Flat> Flats { get; set; } = new List<Flat>();
}
