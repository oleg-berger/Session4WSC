using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Session4.Models;

public partial class Session4DbContext : DbContext
{
    public Session4DbContext()
    {
    }

    public Session4DbContext(DbContextOptions<Session4DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<OrderItems> OrderItems { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<Parts> Parts { get; set; }

    public virtual DbSet<Suppliers> Suppliers { get; set; }

    public virtual DbSet<TransactionTypes> TransactionTypes { get; set; }

    public virtual DbSet<Warehouses> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Если это Android-эмулятор, используем 10.0.2.2, иначе localhost
            string server = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "127.0.0.1";
        
            string connectionString = $"Server={server};Database=Session4;User ID=oleg;Password=1234;TrustServerCertificate=True;";
        
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItems>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Transactions");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PartId).HasColumnName("PartID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.Part).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.PartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Parts");
        });

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SupplierHeaders");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DestinationWarehouseId).HasColumnName("DestinationWarehouseID");
            entity.Property(e => e.SourceWarehouseId).HasColumnName("SourceWarehouseID");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");

            entity.HasOne(d => d.DestinationWarehouse).WithMany(p => p.OrdersDestinationWarehouse)
                .HasForeignKey(d => d.DestinationWarehouseId)
                .HasConstraintName("FK_Headers_Stocks1");

            entity.HasOne(d => d.SourceWarehouse).WithMany(p => p.OrdersSourceWarehouse)
                .HasForeignKey(d => d.SourceWarehouseId)
                .HasConstraintName("FK_Headers_Stocks");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_Headers_Suppliers");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Headers_TransactionTypes");
        });

        modelBuilder.Entity<Parts>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Suppliers>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TransactionTypes>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Warehouses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Stocks");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
