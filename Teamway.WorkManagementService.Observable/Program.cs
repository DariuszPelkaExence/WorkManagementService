using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Teamway.WorkManagementService.Repository;

namespace Teamway.WorkManagementService.Observable
{
    class Program
    {
        private static ServiceProvider _serviceProvider;
        static void Main(string[] args)
        {
            _serviceProvider = new ServiceCollection()
                .AddTransient<IMessageConsumer, WorkerCreatedMessageConsumer>()
                .AddSingleton<IRepository, Repository.Repository>()
                .BuildServiceProvider();
            var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
            const string queueName = "WorkerServiceManagementQueue";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, true, false, false, null);

                // consumer
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += MessageReceived;
                channel.BasicConsume(queueName, true, consumer);
                Console.ReadLine();
            }
        }

        private static async Task MessageReceived(object sender, BasicDeliverEventArgs @event)
        {
            var message = Encoding.UTF8.GetString(@event.Body.ToArray());

            if (@event.RoutingKey == "WorkerCreated")
            {
                var consumer = _serviceProvider.GetService<IMessageConsumer>();
                consumer.ConsumeMessage(message);
            }
        }
    }
}
