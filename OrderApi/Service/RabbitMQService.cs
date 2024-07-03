//using Newtonsoft.Json;
//using OrderApi.Models;
//using RabbitMQ.Client;
//using System.Text;

//namespace OrderApi.Service
//{
//    public class RabbitMQService
//    {
//        private readonly IConnection _connection;
//        private readonly IModel _channel;

//        public RabbitMQService()
//        {
//            var factory = new ConnectionFactory() { HostName = "localhost" };
//            _connection = factory.CreateConnection();
//            _channel = _connection.CreateModel();
//            _channel.QueueDeclare(queue: "orderQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
//        }

//        public void SendOrder(Order order)
//        {
//            var json = JsonConvert.SerializeObject(order);
//            var body = Encoding.UTF8.GetBytes(json);

//            var properties = _channel.CreateBasicProperties();
//            properties.Persistent = true;

//            _channel.BasicPublish(exchange: "", routingKey: "orderQueue", basicProperties: properties, body: body);

//        }

//        public void Dispose()
//        {
//            _channel.Close();
//            _connection.Close();
//        }
//    }
//}
