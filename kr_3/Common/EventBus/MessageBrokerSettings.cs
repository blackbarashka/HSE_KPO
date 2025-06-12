namespace Common.EventBus
{
    /// <summary>
    /// Класс для хранения настроек брокера сообщений.
    /// </summary>
    public class MessageBrokerSettings
    {
        /// <summary>
        /// Адрес брокера сообщений.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Порт брокера сообщений.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Имя пользователя для подключения к брокеру сообщений.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Пароль для подключения к брокеру сообщений.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Имя виртуального хоста для подключения к брокеру сообщений.
        /// </summary>
        public string PaymentRequestQueue => "payment-requests";
        /// <summary>
        /// Имя очереди для ответов на платежные запросы.
        /// </summary>
        public string PaymentResponseQueue => "payment-responses";
    }
}
