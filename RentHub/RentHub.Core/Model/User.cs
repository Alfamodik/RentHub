using System;
using System.Collections.Generic;

namespace RentHub.Core.Model;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Flat> Flats { get; set; } = new List<Flat>();
}
