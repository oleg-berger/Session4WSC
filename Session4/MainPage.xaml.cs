using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Session4.Models;

namespace Session4;

public class InventoryManagement
{
    public string PartName {get; set;}
    public string TransactionType {get; set;}
    public DateOnly Date {get; set;}
    public decimal Amount {get; set;}
    public string Supplier {get; set;}    
    public string DestinationWarehouse {get; set;}
    public long OrderId { get; set; }
    public long TransactionTypeID { get; set; }
}
public partial class MainPage : ContentPage
{
    public List<InventoryManagement> _part  = new();
    public MainPage()
    {
        InitializeComponent();
        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        using (var db = new Session4DbContext())
        {
            var order = await db.Orders
                .Include(oi => oi.OrderItems)
                .ThenInclude(p => p.Part)
                .Include(oi => oi.TransactionType)
                .Include(oi => oi.DestinationWarehouse)
                .Include(oi => oi.Supplier)
                .ToListAsync();
            
            _part = order.Select(oi => new InventoryManagement
            {
                OrderId = oi.Id,
                TransactionTypeID =  oi.TransactionTypeId,
                PartName = oi.OrderItems.FirstOrDefault().Part.Name ?? "N/A",
                TransactionType = oi.TransactionType.Name,
                Date = oi.Date,
                Amount = oi.OrderItems.FirstOrDefault().Amount,
                DestinationWarehouse = oi?.DestinationWarehouse?.Name ?? "N/A",
                Supplier = oi?.Supplier?.Name ?? "N/A"

            }).ToList();
            
            ListOrders.ItemsSource = _part;
        }
    }

    private void RemoveButton_OnClicked(object? sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button?.CommandParameter as InventoryManagement;

        if (item != null)
        {
            _part.Remove(item);
            ListOrders.ItemsSource = null;
            ListOrders.ItemsSource = _part;
        }
            
    }

    private async void OM_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("OrderManagement");
    }

    private async void WM_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("WearehouseManagement");
    }

    private async void IR_OnClicked(object? sender, EventArgs e)
    {
       await Shell.Current.GoToAsync("InventoryReport");
    }


    private async void EditButton_OnClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is InventoryManagement item)
        {
            if (item.TransactionTypeID == 1)
                await Shell.Current.GoToAsync("OrderManagement");
            else if(item.TransactionTypeID == 2)
                await  Shell.Current.GoToAsync("WearehouseManagement");
        }
    }
}
