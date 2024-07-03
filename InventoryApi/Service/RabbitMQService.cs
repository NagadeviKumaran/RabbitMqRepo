using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using InventoryApi.Data;
using InventoryApi.Models;

namespace InventoryApi.Service
{
    public class RabbitMQService : BackgroundService
    {

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "orderQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonConvert.DeserializeObject<Order>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                    dbContext.Orders.Add(order);
                    dbContext.SaveChanges();
                }

                Console.WriteLine($"Order received and saved: {order.ProductName}, Quantity: {order.Quantity}");
            };

            _channel.BasicConsume(queue: "orderQueue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
