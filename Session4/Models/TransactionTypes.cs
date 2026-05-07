using System;
using System.Collections.Generic;

namespace Session4.Models;

public partial class TransactionTypes
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
}
