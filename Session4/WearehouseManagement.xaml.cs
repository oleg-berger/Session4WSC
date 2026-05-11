using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Session4.Models;

namespace Session4;

public partial class WearehouseManagement : ContentPage
{
    public ObservableCollection<PartListData> SelectedData { get; set; }= new();
    public WearehouseManagement()
    {
        InitializeComponent();
        LoadPicker();
        PartListCollection.ItemsSource = SelectedData;
    }

    private async Task LoadPicker()
    {
        await using (var db = new Session4DbContext())
        {
            var warehouse = await db.Warehouses.ToListAsync();
            SourceWarehousePicker.ItemsSource = warehouse;
            DestinationWarehousePicker.ItemsSource = warehouse;
            
            var partName = await db.Parts.ToListAsync();
            PartNamePicker.ItemsSource = partName;
            
        }
    }

    private async void AddButton_OnClicked(object? sender, EventArgs e)
    {
        var selectedPartName = PartNamePicker.SelectedItem as Parts;
        var selectedBatchNumber = BatchNumberEntry.Text;
        var selectedAmount = AmountEntry.Text;

        if (selectedPartName == null)
        {
            await DisplayAlertAsync("Error", "Part name can't be empty", "OK");
            return;
        }

        if (selectedPartName.BatchNumberHasRequired == true && string.IsNullOrEmpty(selectedBatchNumber))
        {
            await DisplayAlertAsync("Error", "Batch number can't be empty", "OK");
            return;
        }
        
        var finalButch = selectedPartName.BatchNumberHasRequired == true
            ? selectedBatchNumber
            : null;
        
        if (string.IsNullOrEmpty(selectedAmount) || !decimal.TryParse(selectedAmount, out decimal amount) || amount < 0)
        {
            await DisplayAlertAsync("Error", "Amount can't be negative", "OK");
            return;
        }

        var newItem = new PartListData()
        {
            Amount = amount,
            BatchNumber = finalButch ?? "N/A",
            PartName = selectedPartName.Name,
            PartId = selectedPartName.Id,

        };
        SelectedData.Add(newItem);
        AmountEntry.Text = string.Empty;
        BatchNumberEntry.Text = string.Empty;
        PartNamePicker.SelectedItem = null;
    }

    private void RemoveButton_OnClicked(object? sender, EventArgs e)
    {
        var button  = sender as Button;
        var item = button?.CommandParameter as PartListData;
        if (item != null)
        {
            var list = PartListCollection.ItemsSource as ObservableCollection<PartListData>;
            list.Remove(item);
        }
    }

    private async void SubmitButton_OnClicked(object? sender, EventArgs e)
    {
        if (SourceWarehousePicker.SelectedItem == null)
        {
            await  DisplayAlertAsync("Error", "Please select a warehouse", "OK");
            return;
        }

        if (DestinationWarehousePicker.SelectedItem == null)
        {
            await DisplayAlertAsync("Error", "Please select a destination", "OK");
            return;
        }

        if (SourceWarehousePicker.SelectedItem == DestinationWarehousePicker.SelectedItem)
        {
            await DisplayAlertAsync("Error", "Warehouse can't be same", "OK");
            return;
        }
        var selectedSW = SourceWarehousePicker.SelectedItem as Warehouses;
        var selctedDW = DestinationWarehousePicker.SelectedItem as Warehouses;
        using (var db = new Session4DbContext())
        {
            var newOrder = new Orders()
            {
                TransactionTypeId = 2,
                Date = DateOnly.FromDateTime(DateTime.Now),
                SupplierId = null,
                SourceWarehouseId = selectedSW.Id,
                DestinationWarehouseId = selctedDW.Id,
            };
            
            db.Orders.Add(newOrder);
            await db.SaveChangesAsync();

            foreach (var parts in SelectedData)
            {
                db.OrderItems.Add(new OrderItems
                {
                    PartId = parts.PartId,
                    Amount = parts.Amount,
                    BatchNumber = parts.BatchNumber,
                    OrderId = newOrder.Id
                });
            }

            await db.SaveChangesAsync();
        }
        await DisplayAlertAsync("Success", "Order added", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void CancelButton_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}