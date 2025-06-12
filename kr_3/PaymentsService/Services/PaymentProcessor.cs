// Services/PaymentProcessor.cs
using Common.Messages;
using PaymentsService.Services;

namespace PaymentsService.Services
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<PaymentProcessor> _logger;

        public PaymentProcessor(
            IAccountService accountService,
            ILogger<PaymentProcessor> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task ProcessPaymentAsync(ProcessPaymentCommand command)
        {
            _logger.LogInformation($"Processing payment for order {command.OrderId}");

            await _accountService.ProcessPaymentAsync(
                command.OrderId,
                command.UserId,
                command.Amount);
        }
    }
}
