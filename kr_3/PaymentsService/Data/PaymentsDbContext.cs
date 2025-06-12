using Microsoft.EntityFrameworkCore;
using PaymentsService.Data.Entities;

namespace PaymentsService.Data
{
    /// <summary>
    /// Контекст базы данных для сервиса платежей
    /// </summary>
    public class PaymentsDbContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста базы данных
        /// </summary>
        /// <param name="options"></param>
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// Таблица аккаунтов пользователей
        /// </summary>
        public DbSet<AccountEntity> Accounts { get; set; }
        /// <summary>
        /// Таблица транзакций
        /// </summary>
        public DbSet<TransactionEntity> Transactions { get; set; }
        /// <summary>
        /// Таблица сообщений Outbox для транзакционной обработки
        /// </summary>
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        /// <summary>
        /// Таблица сообщений Inbox для получения уведомлений
        /// </summary>
        public DbSet<InboxMessage> InboxMessages { get; set; }
        /// <summary>
        /// Настройка модели базы данных
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Аккаунт - один для каждого пользователя
            modelBuilder.Entity<AccountEntity>()
                .HasIndex(a => a.UserId)
                .IsUnique();

            // Транзакции связаны с аккаунтами
            modelBuilder.Entity<TransactionEntity>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId);

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
