using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Session4.Models;

namespace Session4;

public class PartListData
{
    public string PartName { get; set; }
    public string BatchNumber { get; set; }
    public decimal Amount { get; set; }
    
}
public partial class OrderManagement : ContentPage
{
    public OrderManagement()
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
            var partName = await db.Parts.ToListAsync();
            PartNamePicker.ItemsSource = partName;
            var supplier = await db.Suppliers.ToListAsync();
            SupplierPicker.ItemsSource = supplier;
            var warehouse = await db.Warehouses.ToListAsync();
            WarehousePicker.ItemsSource = warehouse;
        }
    }
}