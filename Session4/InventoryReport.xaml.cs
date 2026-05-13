using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Session4.Models;

namespace Session4;

public class BatchNumberResult
{
    public string BatchNumber { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal ReceivedStock { get; set; }
}

public class InventoryReportResult
{
    public long PartId { get; set; }
    public string PartName {get; set;}
    public decimal ReceivedStock {get; set;}
    public decimal CurrentStock {get; set;}
}
public partial class InventoryReport : ContentPage
{
    public InventoryReport()
    {
        InitializeComponent();
        
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await LoadPicker();

      
            

        
    }

    public async Task LoadPicker()
    {
        using (var db = new Session4DbContext())
        {
            var warehouse = await db.Warehouses.ToListAsync();
            WarehousePicker.ItemsSource = warehouse;
        }
    }

    private async void ViewButchNumBut_OnClicked(object? sender, EventArgs e)
    { 
        if (sender is Button button && button.CommandParameter is InventoryReportResult item)
        {
        var selectedWarehouse = WarehousePicker.SelectedItem as Warehouses;

        using var db = new Session4DbContext();
        var orders = await db.Orders
            .Include(o => o.OrderItems)
            .ToListAsync();

        // все batch numbers для этой детали
        var batches = orders
            .Where(o => o.TransactionTypeId == 1 
                     && o.DestinationWarehouseId == selectedWarehouse.Id)
            .SelectMany(o => o.OrderItems)
            .Where(i => i.PartId == item.PartId && i.BatchNumber != null)
            .GroupBy(i => i.BatchNumber)
            .Select(g =>
            {
                var transferOut = orders
                    .Where(o => o.TransactionTypeId == 2 
                             && o.SourceWarehouseId == selectedWarehouse.Id)
                    .SelectMany(o => o.OrderItems)
                    .Where(i => i.PartId == item.PartId && i.BatchNumber == g.Key)
                    .Sum(i => i.Amount);

                var transferIn = orders
                    .Where(o => o.TransactionTypeId == 2 
                             && o.DestinationWarehouseId == selectedWarehouse.Id)
                    .SelectMany(o => o.OrderItems)
                    .Where(i => i.PartId == item.PartId && i.BatchNumber == g.Key)
                    .Sum(i => i.Amount);

                var received = g.Sum(i => i.Amount);

                return new BatchNumberResult
                {
                    BatchNumber = g.Key,
                    ReceivedStock = received,
                    CurrentStock = received + transferIn - transferOut
                };
            }).ToList();

        // показываем в диалоге
        var message = string.Join("\n", batches.Select(b => 
            $"Batch: {b.BatchNumber} | Current: {b.CurrentStock} | Received: {b.ReceivedStock}"));
        
        await DisplayAlert($"Batch Numbers — {item.PartName}", message, "OK");
    }
    }

    private async void GenerateReport_OnClicked(object? sender, EventArgs e)
    {
        var selectedWarehouse = WarehousePicker.SelectedItem as Warehouses;
        if (selectedWarehouse == null)
        {
            await DisplayAlertAsync("Error", "Please select a warehouse to generate report", "OK");
            return;
        }

        using (var db = new Session4DbContext())
        {
            var order = await db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(p => p.Part)
                .ToListAsync();

            var received = order
                .Where(o => o.TransactionTypeId == 1
                            && o.DestinationWarehouseId == selectedWarehouse.Id)
                .SelectMany(o => o.OrderItems)
                .GroupBy(item => new { item.PartId, item.Part.Name })
                .Select(group => new InventoryReportResult
                {
                    PartId = group.Key.PartId,
                    PartName = group.Key.Name,
                    ReceivedStock = group.Sum(item => item.Amount)
                }).ToList();

            var result = received.Select(received =>
            {
                var transferOut = order
                    .Where(o => o.TransactionTypeId == 2
                                && o.SourceWarehouseId == selectedWarehouse.Id)
                    .SelectMany(o => o.OrderItems)
                    .Where(o => o.PartId == received.PartId)
                    .Sum(item => item.Amount);

                var transferIn = order
                    .Where(o => o.TransactionTypeId == 2
                                && o.DestinationWarehouseId == selectedWarehouse.Id)
                    .SelectMany(o => o.OrderItems)
                    .Where(o => o.PartId == received.PartId)
                    .Sum(item => item.Amount);

                return new InventoryReportResult
                {
                    PartName = received.PartName,
                    PartId = received.PartId,
                    ReceivedStock = received.ReceivedStock,
                    CurrentStock = received.ReceivedStock + transferIn - transferOut
                };
            }).ToList();
            if (CurrentStockButton.IsChecked)
                ResultList.ItemsSource = result.Where(r => r.CurrentStock > 0).ToList();
            else if(ReceivedStockButton.IsChecked)
                ResultList.ItemsSource = result;
            else if(OutOfStockButton.IsChecked)
                ResultList.ItemsSource = result.Where(r => r.CurrentStock == 0).ToList();
        }

        
    }
}