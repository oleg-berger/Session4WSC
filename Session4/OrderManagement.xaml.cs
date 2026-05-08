using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Session4.Models;

namespace Session4;

public class PartListData
{
    public long OrderId {get; set;}
    public long PartId {get; set;}
    public string PartName { get; set; }
    public string BatchNumber { get; set; }
    public decimal Amount { get; set; }
    
}
public partial class OrderManagement : ContentPage
{
    public ObservableCollection<PartListData> SelectedData { get; set; } = new();
    public OrderManagement()
    {
        InitializeComponent();
        PartListCollection.ItemsSource = SelectedData;
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
            var partName = await db.Parts.ToListAsync();
            PartNamePicker.ItemsSource = partName;
            var supplier = await db.Suppliers.ToListAsync();
            SupplierPicker.ItemsSource = supplier;
            var warehouse = await db.Warehouses.ToListAsync();
            WarehousePicker.ItemsSource = warehouse;
        }
    }

    private async void AddButton_OnClicked(object? sender, EventArgs e)
    {
        var selectedPicker = PartNamePicker.SelectedItem as Parts;
        var selectedBatchNumber = BatchNumberEntry.Text;
        var selectedAmountNum = AmountEntry.Text;

        if (selectedPicker == null )
        {
            await DisplayAlertAsync("Error", "Please select a part", "OK");
            return;
        }
        if(selectedPicker.BatchNumberHasRequired == true && string.IsNullOrEmpty(selectedBatchNumber))
        {
            await DisplayAlertAsync("Error", "Please select a batch number", "OK");
            return;
        }
        var finalBatch = selectedPicker.BatchNumberHasRequired == true
            ? selectedBatchNumber
            : null;
        if (string.IsNullOrEmpty(selectedAmountNum) || !decimal.TryParse(selectedAmountNum, out decimal amount) ||
            amount <= 0)
        {
            await DisplayAlertAsync("Error", "Please enter a valid amount", "OK");
            return;
        }

        var newItem = new PartListData()
            {
                PartId = selectedPicker.Id,
                Amount = amount,
                BatchNumber = finalBatch,
                PartName = selectedPicker.Name,
            };
        
        SelectedData.Add(newItem);
        AmountEntry.Text = string.Empty;
        PartNamePicker.SelectedItem = null;
        BatchNumberEntry.Text = string.Empty;
    }
}