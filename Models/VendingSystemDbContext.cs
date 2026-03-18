using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Models;

public partial class VendingSystemDbContext : DbContext
{
    public VendingSystemDbContext()
    {
    }

    public VendingSystemDbContext(DbContextOptions<VendingSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<MachineInventory> MachineInventories { get; set; }

    public virtual DbSet<MachineStatus> MachineStatuses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<ServiceTask> ServiceTasks { get; set; }

    public virtual DbSet<StatusHistory> StatusHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VendingMachine> VendingMachines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=VendingSystemDB;Username=postgres;Password=_");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("companies_pkey");

            entity.ToTable("companies");

            entity.HasIndex(e => e.Inn, "companies_inn_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("inn");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("files_pkey");

            entity.ToTable("files");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath).HasColumnName("file_path");
            entity.Property(e => e.MimeType)
                .HasMaxLength(100)
                .HasColumnName("mime_type");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Files)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("files_uploaded_by_fkey");
        });

        modelBuilder.Entity<MachineInventory>(entity =>
        {
            entity.HasKey(e => new { e.MachineId, e.ProductId }).HasName("machine_inventory_pkey");

            entity.ToTable("machine_inventory");

            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
           entity.Property(e => e.LastUpdated)
    .HasDefaultValueSql("CURRENT_TIMESTAMP")
    .HasColumnType("timestamp with time zone")
    .HasColumnName("last_updated");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(0)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Machine).WithMany(p => p.MachineInventories)
                .HasForeignKey(d => d.MachineId)
                .HasConstraintName("machine_inventory_machine_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.MachineInventories)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("machine_inventory_product_id_fkey");
        });

        modelBuilder.Entity<MachineStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("machine_statuses_pkey");

            entity.ToTable("machine_statuses");

            entity.HasIndex(e => e.Name, "machine_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.CreatedAt, "idx_notifications_created");

            entity.HasIndex(e => e.IsRead, "idx_notifications_read");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.Machine).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notifications_machine_id_fkey");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_types_pkey");

            entity.ToTable("payment_types");

            entity.HasIndex(e => e.Name, "payment_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvgDailySales)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("avg_daily_sales");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MinStockLevel)
                .HasDefaultValue(5)
                .HasColumnName("min_stock_level");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sales_pkey");

            entity.ToTable("sales");

            entity.HasIndex(e => e.SaleDate, "idx_sales_date");

            entity.HasIndex(e => e.MachineId, "idx_sales_machine");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
    .HasDefaultValueSql("CURRENT_TIMESTAMP")
    .HasColumnType("timestamp with time zone")
    .HasColumnName("created_at");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.PaymentTypeId).HasColumnName("payment_type_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleDate)
    .HasDefaultValueSql("CURRENT_TIMESTAMP")
    .HasColumnType("timestamp with time zone")
    .HasColumnName("sale_date");

            entity.HasOne(d => d.Machine).WithMany(p => p.Sales)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sales_machine_id_fkey");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.Sales)
                .HasForeignKey(d => d.PaymentTypeId)
                .HasConstraintName("sales_payment_type_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sales_product_id_fkey");
        });

        modelBuilder.Entity<ServiceTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_tasks_pkey");

            entity.ToTable("service_tasks");

            entity.HasIndex(e => e.ScheduledDate, "idx_tasks_scheduled");

            entity.HasIndex(e => e.Status, "idx_tasks_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActualCompletionDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actual_completion_date");
            entity.Property(e => e.AssignedUserId).HasColumnName("assigned_user_id");
            entity.Property(e => e.CancellationReason).HasColumnName("cancellation_reason");
            entity.Property(e => e.CounterValueAfter).HasColumnName("counter_value_after");
            entity.Property(e => e.CounterValueBefore).HasColumnName("counter_value_before");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.Priority)
                .HasDefaultValue(3)
                .HasColumnName("priority");
            entity.Property(e => e.ReportFileUrl).HasColumnName("report_file_url");
            entity.Property(e => e.ReportText).HasColumnName("report_text");
            entity.Property(e => e.ScheduledDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scheduled_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Новая'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TaskType)
                .HasMaxLength(50)
                .HasColumnName("task_type");
            entity.Property(e => e.TravelTimeMinutes)
                .HasDefaultValue(60)
                .HasColumnName("travel_time_minutes");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AssignedUser).WithMany(p => p.ServiceTasks)
                .HasForeignKey(d => d.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("service_tasks_assigned_user_id_fkey");

            entity.HasOne(d => d.Machine).WithMany(p => p.ServiceTasks)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("service_tasks_machine_id_fkey");
        });

        modelBuilder.Entity<StatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_history_pkey");

            entity.ToTable("status_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.NewStatus)
                .HasMaxLength(100)
                .HasColumnName("new_status");
            entity.Property(e => e.OldStatus)
                .HasMaxLength(100)
                .HasColumnName("old_status");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.StatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("status_history_changed_by_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Fio)
                .HasMaxLength(255)
                .HasColumnName("fio");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PhotoUrl).HasColumnName("photo_url");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("users_company_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<VendingMachine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vending_machines_pkey");

            entity.ToTable("vending_machines");

            entity.HasIndex(e => e.CompanyId, "idx_machines_company");

            entity.HasIndex(e => e.StatusId, "idx_machines_status");

            entity.HasIndex(e => e.InventoryNumber, "vending_machines_inventory_number_key").IsUnique();

            entity.HasIndex(e => e.SerialNumber, "vending_machines_serial_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CalibrationIntervalMonths).HasColumnName("calibration_interval_months");
            entity.Property(e => e.CommissioningDate).HasColumnName("commissioning_date");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
    .HasDefaultValueSql("CURRENT_TIMESTAMP")
    .HasColumnType("timestamp with time zone")
    .HasColumnName("created_at");
            entity.Property(e => e.CurrentCash)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("current_cash");
            entity.Property(e => e.InventoryNumber)
                .HasMaxLength(100)
                .HasColumnName("inventory_number");
            entity.Property(e => e.LastCalibrationDate).HasColumnName("last_calibration_date");
            entity.Property(e => e.LastInventoryDate).HasColumnName("last_inventory_date");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 8)
                .HasColumnName("latitude");
            entity.Property(e => e.LocationAddress).HasColumnName("location_address");
            entity.Property(e => e.Longitude)
                .HasPrecision(11, 8)
                .HasColumnName("longitude");
            entity.Property(e => e.ManufactureDate).HasColumnName("manufacture_date");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(255)
                .HasColumnName("manufacturer");
            entity.Property(e => e.ModelName)
                .HasMaxLength(255)
                .HasColumnName("model_name");
            entity.Property(e => e.ModemId)
                .HasMaxLength(100)
                .HasColumnName("modem_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NextCalibrationDate).HasColumnName("next_calibration_date");
            entity.Property(e => e.OperatingHoursCurrent)
                .HasDefaultValue(0)
                .HasColumnName("operating_hours_current");
            entity.Property(e => e.PaymentTypeId).HasColumnName("payment_type_id");
            entity.Property(e => e.ResourceHoursTotal).HasColumnName("resource_hours_total");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(100)
                .HasColumnName("serial_number");
            entity.Property(e => e.ServiceTimeHours).HasColumnName("service_time_hours");
            entity.Property(e => e.StatusId)
                .HasDefaultValue(1)
                .HasColumnName("status_id");
            entity.Property(e => e.SystemEntryDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("system_entry_date");
            entity.Property(e => e.TotalRevenue)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("total_revenue");
            entity.Property(e => e.UpdatedAt)
    .HasDefaultValueSql("CURRENT_TIMESTAMP")
    .HasColumnType("timestamp with time zone")
    .HasColumnName("updated_at");

            entity.HasOne(d => d.Company).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("vending_machines_company_id_fkey");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.PaymentTypeId)
                .HasConstraintName("vending_machines_payment_type_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.VendingMachines)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("vending_machines_status_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
