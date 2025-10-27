<<<<<<< Updated upstream
﻿namespace RentHub.Core.Model;
=======
﻿using System;
using System.Collections.Generic;

namespace RentHub.Core.Model;
>>>>>>> Stashed changes

public partial class Flat
{
    public int FlatId { get; set; }

    public int UserId { get; set; }

    public string Country { get; set; } = null!;

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public string HouseNumber { get; set; } = null!;

    public string ApartmentNumber { get; set; } = null!;

    public int RoomCount { get; set; }

    public decimal Size { get; set; }

    public int FloorNumber { get; set; }

    public int? FloorsNumber { get; set; }

    public string Description { get; set; } = null!;

    public byte[]? Photo { get; set; }

    public virtual ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    public virtual User User { get; set; } = null!;
}
