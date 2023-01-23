namespace MessageBus.RabbitMq
{
    public sealed class RabbitMqOptions
    {
        public string Hostname { get; set; } = default!;
        public string QueueName { get; set; } = default!;
        public int Port { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}