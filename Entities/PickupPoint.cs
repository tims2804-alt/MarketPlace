using System;
using System.Collections.Generic;

namespace MarketPlace2.Entities;

public partial class PickupPoint
{
    public int PickupPointId { get; set; }

    public string Address { get; set; } = null!;

    public string? OpeningHours { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
