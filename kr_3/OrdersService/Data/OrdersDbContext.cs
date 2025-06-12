using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Common.Models;
using OrdersService.Data.Entities;

namespace OrdersService.Data
{
    /// <summary>
    /// Контекст базы данных для управления заказами
    /// </summary>
    public class OrdersDbContext : DbContext
    {
        /// Конструктор контекста базы данных
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// Набор заказов
        /// </summary>
        public DbSet<OrderEntity> Orders { get; set; }
        /// <summary>
        /// Набор сообщений Outbox для отправки
        /// </summary>
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        /// <summary>
        /// Набор сообщений Inbox для получения
        /// </summary>
        public DbSet<InboxMessage> InboxMessages { get; set; }
        /// <summary>
        /// Настройка модели базы данных
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Индекс по ID пользователя для быстрого поиска заказов пользователя
            modelBuilder.Entity<OrderEntity>()
                .HasIndex(o => o.UserId);

            // Индексы для Inbox/Outbox
            modelBuilder.Entity<InboxMessage>()
                .HasIndex(m => m.MessageId)
                .IsUnique();

            modelBuilder.Entity<OutboxMessage>()
                .HasIndex(m => m.MessageId)
                .IsUnique();
        }
    }
}
