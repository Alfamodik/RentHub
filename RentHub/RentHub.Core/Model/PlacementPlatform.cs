using System;
using System.Collections.Generic;

namespace RentHub.Core.Model;

public partial class PlacementPlatform
{
    public int PlatformId { get; set; }

    public string PlatformName { get; set; } = null!;

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
}
