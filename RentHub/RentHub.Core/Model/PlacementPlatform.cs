using System;
using System.Collections.Generic;

namespace RentHub.Core.Model;

public partial class PlacementPlatform
{
    public int IdPlatform { get; set; }

    public string PlatformName { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
