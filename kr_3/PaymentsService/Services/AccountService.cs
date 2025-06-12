// Services/AccountService.cs
using Common.Messages;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Data.Entities;
using PaymentsService.Services;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PaymentsService.Services
{
    public class AccountService : IAccountService
    {
        private readonly PaymentsDbContext _dbContext;
        private readonly ILogger<AccountService> _logger;

        public AccountService(PaymentsDbContext dbContext, ILogger<AccountService> logger) // Обновите конструктор
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Существующие методы остаются без изменений
        public async Task<Account> CreateAccountAsync(string userId)
        {
            // Добавим логирование
            _logger.LogInformation($"Попытка создать счет для пользователя {userId}");

            // Проверяем, нет ли уже аккаунта для этого пользователя
            var existingAccount = await _dbContext.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (existingAccount != null)
            {
                _logger.LogWarning($"Счет для пользователя {userId} уже существует");
                throw new InvalidOperationException("Account already exists for this user");
            }

            var now = DateTime.UtcNow;
            var account = new AccountEntity
            {
                UserId = userId,
                Balance = 0,
                CreatedAt = now,
                UpdatedAt = now
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Счет для пользователя {userId} успешно создан");

            return new Account
            {
                UserId = account.UserId,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        public async Task<Account> TopUpAccountAsync(string userId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount must be positive");
            }

            try
            {
                // Находим аккаунт
                var account = await _dbContext.Accounts
                    .FirstOrDefaultAsync(a => a.UserId == userId);

                if (account == null)
                {
                    throw new InvalidOperationException($"Account not found for user {userId}");
                }

                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation($"Начало пополнения счета: UserId={userId}, Amount={amount}, CurrentBalance={account.Balance}");

                    // Увеличиваем баланс
                    account.Balance += amount;
                    account.UpdatedAt = DateTime.UtcNow;

                    // Создаем транзакцию, ОБЯЗАТЕЛЬНО задаем ReferenceId
                    var transactionEntity = new TransactionEntity
                    {
                        AccountId = account.Id,
                        ReferenceId = $"topup-{Guid.NewGuid()}", // Генерируем уникальный ID для операции пополнения
                        Type = TransactionType.Deposit,
                        Amount = amount,
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.Transactions.Add(transactionEntity);

                    // Сохраняем изменения
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Счет успешно пополнен: UserId={userId}, Amount={amount}, NewBalance={account.Balance}");

                    return new Account
                    {
                        UserId = account.UserId,
                        Balance = account.Balance,
                        CreatedAt = account.CreatedAt,
                        UpdatedAt = account.UpdatedAt
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException($"Failed to top up account: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при работе со счетом: {ex.Message}");
                throw;
            }
        }




        public async Task<Account> GetAccountAsync(string userId)
        {
            var account = await _dbContext.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                return null;
            }

            return new Account
            {
                UserId = account.UserId,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        public async Task<bool> ProcessPaymentAsync(string orderId, string userId, decimal amount)
        {
            // Реализация exactly once семантики с использованием Idempotent Consumer
            var existingInbox = await _dbContext.InboxMessages
                .FirstOrDefaultAsync(m => m.MessageId == orderId);

            if (existingInbox?.ProcessedAt != null)
            {
                // Уже обработали это сообщение
                return true;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var account = await _dbContext.Accounts
                    .FirstOrDefaultAsync(a => a.UserId == userId);

                if (account == null)
                {
                    // Сохраняем событие с ошибкой в outbox
                    await SavePaymentResponseOutbox(orderId, false, "Account not found");
                    await transaction.CommitAsync();
                    return false;
                }

                if (account.Balance < amount)
                {
                    // Сохраняем событие с ошибкой в outbox
                    await SavePaymentResponseOutbox(orderId, false, "Insufficient funds");
                    await transaction.CommitAsync();
                    return false;
                }

                // Вычитаем деньги
                account.Balance -= amount;
                account.UpdatedAt = DateTime.UtcNow;

                // Сохраняем транзакцию
                var transactionEntity = new TransactionEntity
                {
                    AccountId = account.Id,
                    ReferenceId = orderId,
                    Type = TransactionType.Withdrawal,
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Transactions.Add(transactionEntity);

                // Сохраняем идемпотентный inbox
                if (existingInbox == null)
                {
                    _dbContext.InboxMessages.Add(new InboxMessage
                    {
                        MessageId = orderId,
                        MessageType = "ProcessPayment",
                        Content = JsonSerializer.Serialize(new { OrderId = orderId, UserId = userId, Amount = amount }),
                        ReceivedAt = DateTime.UtcNow,
                        ProcessedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    existingInbox.ProcessedAt = DateTime.UtcNow;
                }

                // Сохраняем событие успешной оплаты в outbox
                await SavePaymentResponseOutbox(orderId, true, null);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task SavePaymentResponseOutbox(string orderId, bool success, string failureReason)
        {
            var paymentEvent = new PaymentProcessedEvent
            {
                OrderId = orderId,
                IsSuccess = success,
                FailureReason = failureReason
            };

            _dbContext.OutboxMessages.Add(new OutboxMessage
            {
                MessageId = $"payment-{orderId}-{Guid.NewGuid()}",
                MessageType = nameof(PaymentProcessedEvent),
                Content = JsonSerializer.Serialize(paymentEvent),
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
