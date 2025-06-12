using Common.EventBus;
using Microsoft.Extensions.Options;
using Common.Messages;
using PaymentsService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PaymentsService.Messaging
{
    /// <summary>
    /// Сервис для обработки платежных запросов из RabbitMQ.
    /// </summary>
    public class PaymentRequestConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentRequestConsumer> _logger;
        private readonly MessageBrokerSettings _settings;
        private IConnection _connection;
        private IModel _channel;
        /// <summary>
        /// Инициализирует новый экземпляр класса PaymentRequestConsumer.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public PaymentRequestConsumer(
            IServiceProvider serviceProvider,
            IOptions<MessageBrokerSettings> options,
            ILogger<PaymentRequestConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = options.Value;

            InitializeRabbitMqListener();
        }
        /// <summary>
        /// Инициализирует соединение и канал RabbitMQ для прослушивания платежных запросов.    
        /// </summary>
        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _settings.PaymentRequestQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        /// <summary>
        /// Основной метод для выполнения фоновой задачи по обработке платежных запросов.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Payment Request Consumer started");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var paymentCommand = JsonSerializer.Deserialize<ProcessPaymentCommand>(message);

                    _logger.LogInformation($"Received payment request for Order: {paymentCommand.OrderId}");

                    using var scope = _serviceProvider.CreateScope();
                    var paymentProcessor = scope.ServiceProvider.GetRequiredService<IPaymentProcessor>();

                    await paymentProcessor.ProcessPaymentAsync(paymentCommand);

                    // Подтверждаем сообщение только если успешно его обработали
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment request");

                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: _settings.PaymentRequestQueue,
                autoAck: false,
                consumer: consumer);

            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
