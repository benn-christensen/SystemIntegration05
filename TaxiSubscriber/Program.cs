using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TaxiSubscriber.Model;

namespace TaxiSubscriber
{
    internal class Program
    {
        private static readonly List<Order> _orders = [];
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "orders", type: ExchangeType.Fanout);
            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            string queueName = queueDeclareResult.QueueName;
            await channel.QueueBindAsync(queue: queueName, exchange: "orders", routingKey: string.Empty);
            Console.WriteLine("Venter på order.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Order? order = JsonSerializer.Deserialize<Order>(message);
                Console.WriteLine($"Ny order modtaget");
                if (order is not null)
                {
                    _orders.Add(order);
                    PrintOrders();
                }

                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
            var resultTask = GetUserInput();
            while (true)
            {
                if (resultTask.IsCompleted)
                {

                    resultTask = GetUserInput();
                }
                Thread.Sleep(100);
            }

        }

        private static void PrintOrders()
        {
            Console.WriteLine("Id  | Destination | Afhentnings tidspunkt");
            Console.WriteLine("-----------------------------------------");
            _orders.ForEach(order =>
            {
                string? pickUpTime = order.QuickOrder ? "Snarest muligt" : order.PickUpTime.ToString();
                Console.WriteLine($"{order.Id} | {order.Destination} | {pickUpTime}");
            });
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Vælg en order ved at indtaste id'et");
        }

        private static Task GetUserInput()
        {
            return Task.Run(() =>
            {
                string? input = Console.ReadLine();
                if (input is not null)
                {
                    Console.WriteLine($"Bruger input {input}");
                    _orders.RemoveAll(order => order.Id == input);
                    PrintOrders();
                }
            });
        }
    }
}
