using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Models;
using BCrypt.Net;
using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Server.Data
{
    /// <summary>
    /// Контекст базы данных для приложения
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Таблица пользователей
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Таблица заявок
        /// </summary>
        public DbSet<Request> Requests { get; set; }

        /// <summary>
        /// Таблица наименований
        /// </summary>
        public DbSet<Item> Items { get; set; }

        /// <summary>
        /// Таблица единиц техники (назначений)
        /// </summary>
        public DbSet<Equipment> Equipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка отношений между сущностями
            modelBuilder.Entity<Request>()
                .HasMany(r => r.Items)
                .WithOne(i => i.Request)
                .HasForeignKey(i => i.RequestId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление наименований при удалении заявки

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Equipment)
                .WithMany()
                .HasForeignKey(r => r.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict); // Запрет удаления техники, если она используется в заявке

            // Начальная загрузка данных: администратор
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("0199118822773301"),
                    Role = UserRole.Administrator
                }
            );

            // Опционально: индексы для оптимизации запросов
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Request>()
                .HasIndex(r => r.Number);
        }
    }
}