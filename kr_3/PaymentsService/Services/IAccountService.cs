// Services/IAccountService.cs
using Common.Models;

namespace PaymentsService.Services
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(string userId);
        Task<Account> TopUpAccountAsync(string userId, decimal amount);
        Task<Account> GetAccountAsync(string userId);
        Task<bool> ProcessPaymentAsync(string orderId, string userId, decimal amount);
    }
}
