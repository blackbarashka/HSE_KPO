// Services/IPaymentProcessor.cs
using Common.Messages;


namespace PaymentsService.Services
{
    public interface IPaymentProcessor
    {
        Task ProcessPaymentAsync(ProcessPaymentCommand command);
    }
}
