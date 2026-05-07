using System;
using System.Collections.Generic;

namespace Session4.Models;

public partial class Parts
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long? EffectiveLife { get; set; }

    public bool? BatchNumberHasRequired { get; set; }

    public long? MinimumAmount { get; set; }

    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
}
