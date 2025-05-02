using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Nomenclature> Nomenclature { get; set; }
        public DbSet<DefectGroup> DefectGroups { get; set; }
        public DbSet<Defect> Defects { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Incoming> Incoming { get; set; }
        public DbSet<UserLastSelection> UserLastSelections { get; set; }
        public DbSet<NomenclatureDefectMapping> NomenclatureDefectMappings { get; set; }
        public DbSet<Commissions> Commissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commissions>(
                entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.HasOne(e => e.Approve)
                        .WithMany() 
                        .HasForeignKey(e => e.ApproveId)
                        .OnDelete(DeleteBehavior.Restrict);
                    entity.HasOne(e => e.Chairman)
                        .WithMany()
                        .HasForeignKey(e => e.ChairmanId)
                        .OnDelete(DeleteBehavior.Restrict);
                    entity.HasOne(e => e.Member1)
                        .WithMany()
                        .HasForeignKey(e => e.Member1Id)
                        .OnDelete(DeleteBehavior.Restrict);
                    entity.HasOne(e => e.Member2)
                        .WithMany()
                        .HasForeignKey(e => e.Member2Id)
                        .OnDelete(DeleteBehavior.Restrict);
                    entity.HasOne(e => e.Member3)
                        .WithMany()
                        .HasForeignKey(e => e.Member3Id)
                        .OnDelete(DeleteBehavior.Restrict);
                    entity.HasOne(e => e.Member4)
                        .WithMany()
                        .HasForeignKey(e => e.Member4Id)
                        .OnDelete(DeleteBehavior.Restrict);
                }
            );
            modelBuilder.Entity<UserLastSelection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Driver)
                    .WithMany()
                    .HasForeignKey(e => e.DriverId)
                    .IsRequired(false);
                entity.HasOne(e => e.Equipment)
                    .WithMany()
                    .HasForeignKey(e => e.EquipmentId)
                    .IsRequired(false);
            });
            modelBuilder.Entity<NomenclatureDefectMapping>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Nomenclature)
                    .WithMany()
                    .HasForeignKey(e => e.NomenclatureId);
                entity.HasOne(e => e.Defect)
                    .WithMany()
                    .HasForeignKey(e => e.DefectId);
            });
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Ignore(e => e.IsSelected);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.Date)
                    .HasColumnType("timestamp with time zone");
                entity.HasOne(e => e.Stock)
                    .WithMany() 
                    .HasForeignKey(e => e.StockId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Ignore(e=>e.IsSelected);
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Date)
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(e => e.Stock)
                    .WithMany() // Если нужно, можно добавить коллекцию Expenses в Stock
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
            });
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.InitialQuantity).HasColumnType("decimal(18,2)");
                entity.Property(s => s.ReceivedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(s => s.ConsumedQuantity).HasColumnType("decimal(18,2)");

                entity.HasOne(s => s.Warehouse)
                    .WithMany(w => w.Stocks)
                    .HasForeignKey(s => s.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict); // Запрет удаления склада, если есть запасы

                entity.HasOne(s => s.Nomenclature)
                    .WithMany(n => n.Stocks)
                    .HasForeignKey(s => s.NomenclatureId)
                    .OnDelete(DeleteBehavior.Restrict); // Запрет удаления номенклатуры, если есть запасы
            });
            // Отношения для DefectGroup и Defect
            modelBuilder.Entity<DefectGroup>()
                .HasMany(dg => dg.Defects)
                .WithOne(d => d.DefectGroup)
                .HasForeignKey(d => d.DefectGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = 1,
                    StockId = 1,
                    Quantity = 5,
                    EquipmentId = 1,
                    DriverId = 1,
                    DefectId = 1,
                    Date = DateTime.SpecifyKind(DateTime.Parse("12.04.2025"), DateTimeKind.Utc)
                }
            );
            modelBuilder.Entity<Incoming>().HasData(
                new Incoming
                {
                    Id = 1,
                    StockId = 1,
                    Quantity = 5,
                    Date = DateTime.SpecifyKind(DateTime.Parse("15.04.2025"), DateTimeKind.Utc)
                });
            // Начальные данные для Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    Password = "$2a$11$IeKuyvG/5SoDYP0NFz3kouC3CPUIuUa6ShTfgVVf9oUlfqbXq8LrC",
                    Role = UserRole.Administrator
                }
            );

            // Начальные данные для Warehouses
            modelBuilder.Entity<Warehouse>().HasData(
                new Warehouse { Id = 1, Name = "Основной склад" },
                new Warehouse { Id = 2, Name = "Резервный склад" }
            );

            // Начальные данные для Nomenclature
            modelBuilder.Entity<Nomenclature>().HasData(
                new Nomenclature
                {
                    Id = 1,
                    Code = "ТКР001",
                    Name = "Турбокомпрессор ТКР 7С-6 левый КАМАЗ Евро 2",
                    Article = "7406.1118013",
                    UnitOfMeasure = "шт",
                },
                new Nomenclature
                {
                    Id = 2,
                    Code = "АКБ001",
                    Name = "Аккумулятор 6СТ-190",
                    Article = "6СТ-190",
                    UnitOfMeasure = "шт",
                },
                new Nomenclature
                {
                    Id = 3,
                    Code = "АКБ002",
                    Name = "Аккумулятор 6СТ-200 (аналог 6СТ-190)",
                    Article = "6СТ-200",
                    UnitOfMeasure = "шт",
                }
            );
            modelBuilder.Entity<Stock>().HasData(
                new Stock
                {
                    Id = 1,
                    NomenclatureId = 1,
                    WarehouseId = 1,
                    InitialQuantity = 70,
                    ReceivedQuantity = 0,
                    ConsumedQuantity = 0
                },
                new Stock
                {
                    Id = 2,
                    NomenclatureId = 2,
                    WarehouseId = 1,
                    InitialQuantity = 10,
                    ReceivedQuantity = 0,
                    ConsumedQuantity = 0
                },
            new Stock
            {
                Id = 3,
                NomenclatureId = 1,
                WarehouseId = 2,
                InitialQuantity = 40,
                ReceivedQuantity = 0,
                ConsumedQuantity = 0
            },
            new Stock
            {
                Id = 4,
                NomenclatureId = 2,
                WarehouseId = 2,
                InitialQuantity = 20,
                ReceivedQuantity = 0,
                ConsumedQuantity = 0
            }
            );

            // Начальные данные для Drivers
            modelBuilder.Entity<Driver>().HasData(
                new Driver
                {
                    Id = 1,
                    FullName = "Иванов Иван Иванович",
                    ShortName = "Иванов И.И.",
                    Position = "Водитель"
                },
                new Driver
                {
                    Id = 2,
                    FullName = "Петров Петр Петрович",
                    ShortName = "Петров П.П.",
                    Position = "Водитель"
                }
            );

            // Начальные данные для Equipment
            modelBuilder.Entity<Equipment>().HasData(
                new Equipment
                {
                    Id = 1,
                    Name = "КАМАЗ 53215-15",
                    StateNumber = "Н 507 СН"
                }
            );

            // Начальные данные для DefectGroups
            modelBuilder.Entity<DefectGroup>().HasData(
                new DefectGroup { Id = 1, Name = "Механические повреждения" },
                new DefectGroup { Id = 2, Name = "Электрические неисправности" }
            );

            // Начальные данные для Defects
            modelBuilder.Entity<Defect>().HasData(
                new Defect { Id = 1, Name = "Трещина корпуса", DefectGroupId = 1 },
                new Defect { Id = 2, Name = "Короткое замыкание", DefectGroupId = 2 }
            );

            // Индексы
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();
        }
    }
}