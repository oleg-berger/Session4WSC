using System;
using System.Collections.Generic;

namespace Session4.Models;

public partial class OrderItems
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long PartId { get; set; }

    public string? BatchNumber { get; set; }

    public decimal Amount { get; set; }

    public virtual Orders Order { get; set; } = null!;

    public virtual Parts Part { get; set; } = null!;
}
