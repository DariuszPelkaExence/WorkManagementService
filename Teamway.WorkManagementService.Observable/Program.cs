using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Teamway.WorkManagementService.Observable
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
            const string queueName = "WorkerServiceManagement";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, true, false, false, null);

                // consumer
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += ConsumerReceived;
                channel.BasicConsume(queueName, true, consumer);
            }
        }

        private static async Task ConsumerReceived(object sender, BasicDeliverEventArgs @event)
        {
            var message = Encoding.UTF8.GetString(@event.Body.ToArray());

        }
    }
}
