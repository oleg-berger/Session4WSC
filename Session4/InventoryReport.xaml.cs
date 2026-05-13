using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Session4.Models;

namespace Session4;

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

    private void ViewButchNumBut_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
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