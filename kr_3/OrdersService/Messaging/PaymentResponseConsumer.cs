// Messaging/PaymentResponseConsumer.cs
using Common.EventBus;
using Microsoft.Extensions.Options;
using Common.Messages;
using OrdersService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;



namespace OrdersService.Messaging
{
    /// <summary>
    /// Сервис для обработки ответов на платежи из RabbitMQ
    /// </summary>
    public class PaymentResponseConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentResponseConsumer> _logger;
        private readonly MessageBrokerSettings _settings;
        private IConnection _connection;
        private IModel _channel;
        /// <summary>
        /// Инициализирует новый экземпляр класса PaymentResponseConsumer.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public PaymentResponseConsumer(
            IServiceProvider serviceProvider,
            IOptions<MessageBrokerSettings> options,
            ILogger<PaymentResponseConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = options.Value;

            InitializeRabbitMqListener();
        }
        /// <summary>
        /// Инициализирует соединение и канал RabbitMQ для прослушивания ответов на платежи.
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
                queue: _settings.PaymentResponseQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        /// <summary>
        /// Основной метод для запуска фонового сервиса, который обрабатывает ответы на платежи.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Payment Response Consumer started");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(message);

                    _logger.LogInformation($"Received payment response for Order: {paymentEvent.OrderId}, Success: {paymentEvent.IsSuccess}");

                    using var scope = _serviceProvider.CreateScope();
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    await orderService.UpdateOrderStatusAsync(
                        paymentEvent.OrderId,
                        paymentEvent.IsSuccess,
                        paymentEvent.FailureReason);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment response");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: _settings.PaymentResponseQueue,
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
