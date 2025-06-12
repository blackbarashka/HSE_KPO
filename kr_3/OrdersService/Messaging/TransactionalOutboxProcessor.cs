using Common.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrdersService.Data;
using RabbitMQ.Client;
using System.Text;

namespace OrdersService.Messaging
{
    /// <summary>
    /// Сервис для обработки сообщений из Outbox и отправки их в RabbitMQ.
    /// </summary>
    public class TransactionalOutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransactionalOutboxProcessor> _logger;
        private readonly MessageBrokerSettings _settings;
        private IConnection _connection;
        private IModel _channel;
        /// <summary>
        /// Инициализирует новый экземпляр класса TransactionalOutboxProcessor.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public TransactionalOutboxProcessor(
            IServiceProvider serviceProvider,
            IOptions<MessageBrokerSettings> options,
            ILogger<TransactionalOutboxProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = options.Value;
            InitializeRabbitMq();
        }
        /// <summary>
        /// Инициализирует соединение и канал RabbitMQ для отправки сообщений.
        /// </summary>
        private void InitializeRabbitMq()
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
        /// Основной метод для выполнения фоновой задачи по обработке Outbox сообщений.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Transactional Outbox Processor started");

            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessOutboxMessagesAsync();

                // Задежка.
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        /// <summary>
        /// Обрабатывает сообщения из Outbox и отправляет их в RabbitMQ.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessOutboxMessagesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.Id)
                .Take(10)
                .ToListAsync();

            if (!messages.Any())
            {
                return;
            }

            foreach (var message in messages)
            {
                try
                {
                    var body = Encoding.UTF8.GetBytes(message.Content);

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: _settings.PaymentRequestQueue,
                        basicProperties: null,
                        body: body);

                    message.ProcessedAt = DateTime.UtcNow;

                    _logger.LogInformation($"Published message {message.MessageId} to {_settings.PaymentRequestQueue}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to publish message {message.MessageId}");
                }
            }

            await dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Освобождает ресурсы, используемые сервисом.
        /// </summary>
        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
