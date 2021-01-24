using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.API
{
    public class MessagePublisher : IMessagePublisher
    {
        private bool _isTurned = false;

        public Task<int> SendMessageShiftCreated(Shift shift)
        {
            Console.WriteLine("Shift created");
            // here we should send shift created message ...
            if (_isTurned)
            {
                try
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: "ShiftCreated",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            String jsonified = JsonConvert.SerializeObject(shift);
                            byte[] customerBuffer = Encoding.UTF8.GetBytes(jsonified);
                            channel.BasicPublish(exchange: "",
                                routingKey: "ShiftCreated",
                                basicProperties: null,
                                body: customerBuffer);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }
            }
            return Task.FromResult(0);
        }

        public Task<int> SendMessageShiftRemoved(Shift shift)
        {
            Console.WriteLine("Shift removed");
            // here we should send shift removed message ...

            if (_isTurned)
            {
                try
                {
                    var factory = new ConnectionFactory() {HostName = "localhost"};
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: "ShiftRemoved",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            String jsonified = JsonConvert.SerializeObject(shift);
                            byte[] customerBuffer = Encoding.UTF8.GetBytes(jsonified);
                            channel.BasicPublish(exchange: "",
                                routingKey: "ShiftRemoved",
                                basicProperties: null,
                                body: customerBuffer);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }
            }

            return Task.FromResult(0);
        }
    }
}
