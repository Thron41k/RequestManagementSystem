using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Models;
using BCrypt.Net;
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
        public DbSet<NomenclatureAnalog> NomenclatureAnalogs { get; set; }
        public DbSet<Consumption> Consumptions { get; set; }
        public DbSet<DefectGroup> DefectGroups { get; set; }
        public DbSet<Defect> Defects { get; set; }
        public DbSet<ConsumptionItem> ConsumptionItems { get; set; }
        public DbSet<Driver> Drivers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Отношения для NomenclatureAnalogs
            modelBuilder.Entity<NomenclatureAnalog>()
                .HasKey(na => na.Id);

            modelBuilder.Entity<NomenclatureAnalog>()
                .HasOne(na => na.MainNomenclature)
                .WithMany()
                .HasForeignKey(na => na.MainNomenclatureId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NomenclatureAnalog>()
                .HasOne(na => na.AnalogNomenclature)
                .WithMany()
                .HasForeignKey(na => na.AnalogNomenclatureId)
                .OnDelete(DeleteBehavior.Restrict);

            // Отношения для Consumption
            modelBuilder.Entity<Consumption>()
                .HasOne(c => c.Warehouse)
                .WithMany()
                .HasForeignKey(c => c.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Consumption>()
                .HasOne(c => c.Equipment)
                .WithMany()
                .HasForeignKey(c => c.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Consumption>()
                .HasOne(c => c.Driver)
                .WithMany()
                .HasForeignKey(c => c.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Consumption>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Consumption)
                .HasForeignKey(ci => ci.ConsumptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Отношения для DefectGroup и Defect
            modelBuilder.Entity<DefectGroup>()
                .HasMany(dg => dg.Defects)
                .WithOne(d => d.DefectGroup)
                .HasForeignKey(d => d.DefectGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Отношения для ConsumptionItem
            modelBuilder.Entity<ConsumptionItem>()
                .HasOne(ci => ci.Nomenclature)
                .WithMany()
                .HasForeignKey(ci => ci.NomenclatureId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsumptionItem>()
                .HasOne(ci => ci.Defect)
                .WithMany()
                .HasForeignKey(ci => ci.DefectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Начальные данные для Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    Password = "$2a$11$abcdefghijk123456789u.lX7Qz5Z9K8zM8zM8zM8zM8zM8zM8zM8zM",
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

            // Начальные данные для NomenclatureAnalogs
            modelBuilder.Entity<NomenclatureAnalog>().HasData(
                new NomenclatureAnalog
                {
                    Id = 1,
                    MainNomenclatureId = 2,
                    AnalogNomenclatureId = 3
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

            // Начальные данные для Consumptions
            modelBuilder.Entity<Consumption>().HasData(
                new Consumption
                {
                    Id = 1,
                    Number = "РСХ0001",
                    Date = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc),
                    WarehouseId = 1,
                    EquipmentId = 1,
                    DriverId = 1
                }
            );

            // Начальные данные для ConsumptionItems
            modelBuilder.Entity<ConsumptionItem>().HasData(
                new ConsumptionItem
                {
                    Id = 1,
                    NomenclatureId = 2, // Аккумулятор 6СТ-190
                    Quantity = 1,
                    DefectId = 2,       // Короткое замыкание
                    ConsumptionId = 1
                }
            );

            // Индексы
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Consumption>()
                .HasIndex(c => c.Number)
                .IsUnique();
        }
    }
}