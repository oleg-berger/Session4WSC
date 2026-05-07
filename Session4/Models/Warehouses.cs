using System;
using System.Collections.Generic;

namespace Session4.Models;

public partial class Warehouses
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Orders> OrdersDestinationWarehouse { get; set; } = new List<Orders>();

    public virtual ICollection<Orders> OrdersSourceWarehouse { get; set; } = new List<Orders>();
}
