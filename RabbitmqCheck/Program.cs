//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System;
//using System.Text;
//using Newtonsoft.Json;
//using System.ComponentModel.DataAnnotations;

//class Program
//{
//    static void Main(string[] args)
//    {
//        var factory = new ConnectionFactory() { HostName = "localhost" }; // Replace with your RabbitMQ host
//        using (var connection = factory.CreateConnection())
//        using (var channel = connection.CreateModel())
//        {
//            channel.QueueDeclare(queue: "orderQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

//            var consumer = new EventingBasicConsumer(channel);
//            consumer.Received += (model, ea) =>
//            {
//                var body = ea.Body.ToArray();
//                var message = Encoding.UTF8.GetString(body);
//                Console.WriteLine("Received message: {0}", message);

//                // Deserialize JSON
//                var product = JsonConvert.DeserializeObject<Order>(message);
//                Console.WriteLine("Deserialized object: Id={0}, ProductName={1}, Quantity={2}", product.Id, product.ProductName, product.Quantity);
//            };
//            channel.BasicConsume(queue: "orderQueue", autoAck: false, consumer: consumer);

//            Console.WriteLine("Press [enter] to exit.");
//            Console.ReadLine();
//        }
//    }
//}

//public class Order
//{
//    public int Id { get; set; }
//    public string ProductName { get; set; }
//    public int Quantity { get; set; }
//}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        // Connection settings
        var factory = new ConnectionFactory() { HostName = "localhost" }; // Replace with your RabbitMQ host

        // Establish a connection and create a channel
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Declare the queue
            channel.QueueDeclare(queue: "orderQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Set up the consumer
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Received message: {0}", message);

                try
                {
                    // Deserialize JSON message to Order object
                    var order = JsonConvert.DeserializeObject<Order>(message);
                    Console.WriteLine("Deserialized object: Id={0}, ProductName={1}, Quantity={2}", order.Id, order.ProductName, order.Quantity);

                    // Manually acknowledge the message
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing message: {0}", ex.Message);

                    // Requeue the message in case of an error
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            // Start consuming messages from the queue
            channel.BasicConsume(queue: "orderQueue", autoAck: false, consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}

public class Order
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}
