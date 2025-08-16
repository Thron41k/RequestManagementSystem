using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // DbSets с правильным именованием (множественное число)
    public DbSet<User> Users { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Nomenclature> Nomenclatures { get; set; }
    public DbSet<DefectGroup> DefectGroups { get; set; }
    public DbSet<EquipmentGroup> EquipmentGroups { get; set; }
    public DbSet<Defect> Defects { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Incoming> Incomings { get; set; }
    public DbSet<UserLastSelection> UserLastSelections { get; set; }
    public DbSet<NomenclatureDefectMapping> NomenclatureDefectMappings { get; set; }
    public DbSet<Commissions> Commissions { get; set; }
    public DbSet<NomenclatureAnalog> NomenclatureAnalogs { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<SparePartsOwnership> SparePartsOwnerships { get; set; }
    public DbSet<MaterialsInUse> MaterialsInUse { get; set; }
    public DbSet<ReasonsForWritingOffMaterialsFromOperation> ReasonsForWritingOffMaterialsFromOperation { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureEquipment(modelBuilder);
        ConfigureWarehouse(modelBuilder);
        ConfigureNomenclature(modelBuilder);
        ConfigureDefectGroup(modelBuilder);
        ConfigureDefect(modelBuilder);
        ConfigureDriver(modelBuilder);
        ConfigureStock(modelBuilder);
        ConfigureExpense(modelBuilder);
        ConfigureIncoming(modelBuilder);
        ConfigureUserLastSelection(modelBuilder);
        ConfigureNomenclatureDefectMapping(modelBuilder);
        ConfigureCommissions(modelBuilder);
        ConfigureNomenclatureAnalog(modelBuilder);
        ConfigureApplication(modelBuilder);
        ConfigureEquipmentGroup(modelBuilder);
        ConfigureEquipmentGroupNomenclature(modelBuilder);
        ConfigureMaterialsInUse(modelBuilder);
        ConfigureReasonsForWritingOffMaterialsFromOperation(modelBuilder);
        SeedInitialData(modelBuilder);
    }

    private void ConfigureMaterialsInUse(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaterialsInUse>(entity =>
        {

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DocumentNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Date)
                .IsRequired();

            entity.Property(e => e.Quantity)
                .HasColumnType("numeric(18,3)")
                .IsRequired();

            entity.Property(e => e.NomenclatureId)
                .IsRequired();

            entity.Property(e => e.EquipmentId)
                .IsRequired();

            entity.HasOne(e => e.Nomenclature)
                .WithMany()
                .HasForeignKey(e => e.NomenclatureId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(e => e.Equipment)
                .WithMany()
                .HasForeignKey(e => e.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(e => e.FinanciallyResponsiblePerson)
                .WithMany()
                .HasForeignKey(e => e.FinanciallyResponsiblePersonId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.ReasonForWriteOff)
                .WithMany()
                .HasForeignKey(e => e.ReasonForWriteOffId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private void ConfigureReasonsForWritingOffMaterialsFromOperation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReasonsForWritingOffMaterialsFromOperation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason)
                .IsRequired();
            entity.HasIndex(e => e.Reason);
        });
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Login)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(256);
            entity.HasIndex(e => e.Login)
                .IsUnique();
        });
    }

    private void ConfigureEquipment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.StateNumber)
                .HasMaxLength(20);
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(e => e.Code);
        });
    }

    private void ConfigureEquipmentGroup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquipmentGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasMany(e => e.Equipments)
                .WithOne(d => d.EquipmentGroup)
                .HasForeignKey(d => d.EquipmentGroupId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
    private void ConfigureWarehouse(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsRequired(false); // Code is nullable in the model
            entity.Property(e => e.LastUpdated)
                .HasColumnType("timestamp with time zone");
            entity.HasOne(e => e.FinanciallyResponsiblePerson)
                .WithMany() // No navigation property in Driver
                .HasForeignKey(e => e.FinanciallyResponsiblePersonId)
                .IsRequired(false) // FinanciallyResponsiblePersonId is nullable
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Stocks)
                .WithOne(s => s.Warehouse)
                .HasForeignKey(s => s.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Name);
        });
    }


    private void ConfigureNomenclature(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nomenclature>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Article)
                .HasMaxLength(100);
            entity.Property(e => e.UnitOfMeasure)
                .IsRequired()
                .HasMaxLength(20);
            entity.HasIndex(e => e.Code);
        });
    }

    private void ConfigureDefectGroup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DefectGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasMany(e => e.Defects)
                .WithOne(d => d.DefectGroup)
                .HasForeignKey(d => d.DefectGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureDefect(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Defect>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(e => e.Name);
        });
    }

    private void ConfigureDriver(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.ShortName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(e => e.Code);
        });
    }

    private void ConfigureStock(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InitialQuantity)
                .HasColumnType("decimal(18,3)");
            entity.Property(e => e.ReceivedQuantity)
                .HasColumnType("decimal(18,3)");
            entity.Property(e => e.ConsumedQuantity)
                .HasColumnType("decimal(18,3)");
            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.Stocks)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Nomenclature)
                .WithMany(n => n.Stocks)
                .HasForeignKey(e => e.NomenclatureId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.WarehouseId, e.NomenclatureId });
        });
    }

    private void ConfigureExpense(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Ignore(e => e.IsSelected);
            entity.Property(e => e.Code)
                .HasMaxLength(50);
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(18,3)");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp with time zone");
            entity.HasOne(e => e.Stock)
                .WithMany()
                .HasForeignKey(e => e.StockId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Equipment)
                .WithMany()
                .HasForeignKey(e => e.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Driver)
                .WithMany()
                .HasForeignKey(e => e.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Defect)
                .WithMany()
                .HasForeignKey(e => e.DefectId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.Date);
        });
    }

    private void ConfigureIncoming(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Incoming>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Ignore(e => e.IsSelected);
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(18,3)");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp with time zone");
            entity.Property(e => e.DocType)
                .IsRequired()
                .HasMaxLength(50); // Added max length for consistency
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasOne(e => e.Stock)
                .WithMany()
                .HasForeignKey(e => e.StockId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Application)
                .WithMany()
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.InWarehouse)
                .WithMany()
                .HasForeignKey(e => e.InWarehouseId)
                .IsRequired(false) // InWarehouseId is nullable
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.Date);
        });
    }

    private void ConfigureUserLastSelection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserLastSelection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LastUpdated)
                .HasColumnType("timestamp with time zone");
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Driver)
                .WithMany()
                .HasForeignKey(e => e.DriverId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Equipment)
                .WithMany()
                .HasForeignKey(e => e.EquipmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Commissions)
                .WithMany()
                .HasForeignKey(e => e.CommissionsId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.UserId);
        });
    }

    private void ConfigureNomenclatureDefectMapping(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NomenclatureDefectMapping>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LastUsed)
                .HasColumnType("timestamp with time zone");
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Nomenclature)
                .WithMany()
                .HasForeignKey(e => e.NomenclatureId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Defect)
                .WithMany()
                .HasForeignKey(e => e.DefectId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.UserId, e.NomenclatureId });
        });
    }
    private void ConfigureApplication(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Number)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Date)
                .HasColumnType("timestamp with time zone");
            entity.HasOne(e => e.Responsible)
                .WithMany()
                .HasForeignKey(e => e.ResponsibleId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Equipment)
                .WithMany()
                .HasForeignKey(e => e.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.Number);
        });
    }
    private void ConfigureCommissions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Commissions>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasOne(e => e.ApproveForAct)
                .WithMany()
                .HasForeignKey(e => e.ApproveForActId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ApproveForDefectAndLimit)
                .WithMany()
                .HasForeignKey(e => e.ApproveForDefectAndLimitId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Chairman)
                .WithMany()
                .HasForeignKey(e => e.ChairmanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Member1)
                .WithMany()
                .HasForeignKey(e => e.Member1Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Member2)
                .WithMany()
                .HasForeignKey(e => e.Member2Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Member3)
                .WithMany()
                .HasForeignKey(e => e.Member3Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Member4)
                .WithMany()
                .HasForeignKey(e => e.Member4Id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.Name);
        });
    }

    private void ConfigureNomenclatureAnalog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NomenclatureAnalog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Original)
                .WithMany()
                .HasForeignKey(e => e.OriginalId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Analog)
                .WithMany()
                .HasForeignKey(e => e.AnalogId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.OriginalId, e.AnalogId });
        });
    }

    private void ConfigureEquipmentGroupNomenclature(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SparePartsOwnership>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.EquipmentGroup)
                .WithMany(g => g.SparePartsOwnerships)
                .HasForeignKey(e => e.EquipmentGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Nomenclature)
                .WithMany(n => n.SparePartsOwnerships)
                .HasForeignKey(e => e.NomenclatureId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.EquipmentGroupId, e.NomenclatureId }).IsUnique(); // предотвращает дубликаты
        });
    }
    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Login = "admin",
                Password = "$2a$11$IeKuyvG/5SoDYP0NFz3kouC3CPUIuUa6ShTfgVVf9oUlfqbXq8LrC",
                Role = UserRole.Administrator
            }
        );
        modelBuilder.Entity<Commissions>().HasData(new Commissions
        {
            Id = 1,
            Name = "Могочинский филиал АО \"Труд\"",
            ApproveForActId = 1,
            ApproveForDefectAndLimitId = 1,
            ChairmanId = 1,
            Member1Id = 1,
            Member2Id = 1,
            Member3Id = 1,
            Member4Id = 1
        });
        modelBuilder.Entity<Nomenclature>().HasData(
            new Nomenclature
            {
                Id = 1,
                Code = "",
                Name = "",
                Article = "",
                UnitOfMeasure = ""
            }
        );

        modelBuilder.Entity<Driver>().HasData(
            new Driver
            {
                Id = 1,
                FullName = "",
                ShortName = "",
                Position = "",
                Code = ""
            }
        );

        modelBuilder.Entity<Equipment>().HasData(
            new Equipment
            {
                Id = 1,
                Name = "",
                StateNumber = "",
                Code = "",
                ShortName = ""
            }
        );

        modelBuilder.Entity<DefectGroup>().HasData(
            new DefectGroup { Id = 1, Name = "" },
            new DefectGroup { Id = 2, Name = "Выпускная система" },
            new DefectGroup { Id = 3, Name = "Гидравлика" },
            new DefectGroup { Id = 4, Name = "ДВС" },
            new DefectGroup { Id = 5, Name = "Коробка раздаточная" },
            new DefectGroup { Id = 6, Name = "Кузов, кабина" },
            new DefectGroup { Id = 7, Name = "Механизмы управления" },
            new DefectGroup { Id = 8, Name = "Рабочее оборудование" },
            new DefectGroup { Id = 9, Name = "Система охлаждения" },
            new DefectGroup { Id = 10, Name = "Сцепление" },
            new DefectGroup { Id = 11, Name = "Топливная система" },
            new DefectGroup { Id = 12, Name = "Трансмиссия" },
            new DefectGroup { Id = 13, Name = "Ходовая часть" },
            new DefectGroup { Id = 14, Name = "Электрооборудование" },
            new DefectGroup { Id = 15, Name = "Расходные материалы" },
            new DefectGroup { Id = 16, Name = "Передача в эксплуатацию" }
        );

        modelBuilder.Entity<Defect>().HasData(
            new Defect { Id = 1, Name = "", DefectGroupId = 1 },
            new Defect { Id = 2, Name = "Замена АКБ", DefectGroupId = 14 },
            new Defect { Id = 3, Name = "Замена автошин", DefectGroupId = 13 },
            new Defect { Id = 4, Name = "Передача в эксплуатацию", DefectGroupId = 16 }
        );

        modelBuilder.Entity<Application>().HasData(
            new Application
            {
                Id = 1,
                Number = "",
                Date = DateTime.SpecifyKind(DateTime.Parse("2025-04-10"), DateTimeKind.Utc),
                ResponsibleId = 1,
                EquipmentId = 1
            }
        );

        modelBuilder.Entity<ReasonsForWritingOffMaterialsFromOperation>().HasData(
            new ReasonsForWritingOffMaterialsFromOperation { Id = 1, Reason = "" }
            );
    }
}