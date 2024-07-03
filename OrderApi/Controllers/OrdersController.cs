using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderApi.Models;
using RabbitMQ.Client;
using System.Text;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public OrdersController()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "orderQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        [HttpPost]
        public IActionResult PostOrder(Order order)
        {
            var json = JsonConvert.SerializeObject(order);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: "", routingKey: "orderQueue", basicProperties: null, body: body);

            return Ok();
        }

        
    }

    
}

