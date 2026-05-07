using System;
using System.Collections.Generic;

namespace Session4.Models;

public partial class Orders
{
    public long Id { get; set; }

    public long TransactionTypeId { get; set; }

    public long? SupplierId { get; set; }

    public long? SourceWarehouseId { get; set; }

    public long? DestinationWarehouseId { get; set; }

    public DateOnly Date { get; set; }

    public virtual Warehouses? DestinationWarehouse { get; set; }

    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

    public virtual Warehouses? SourceWarehouse { get; set; }

    public virtual Suppliers? Supplier { get; set; }

    public virtual TransactionTypes TransactionType { get; set; } = null!;
}
