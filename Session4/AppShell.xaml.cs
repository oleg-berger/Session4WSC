namespace Session4;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("OrderManagement",  typeof(OrderManagement));
        Routing.RegisterRoute("WearehouseManagement",  typeof(WearehouseManagement));
        Routing.RegisterRoute("InventoryReport",  typeof(InventoryReport));
    }
}