using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary2.Services
{
    public class Consumer
    {

        private static readonly Lazy<Consumer> _instance = new Lazy<Consumer>(() => new Consumer());
        private readonly List<string> RoutingKeys;
        private readonly string HostName;
        private readonly int PortNumber;
        private readonly string Exchange;
        private readonly string Queue;
        private const string ConsumerName = "Consumer";
        public IConnection connection { get; private set; }
        private Consumer()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = ConfigurationManager.AppSettings["RabbitMQHostName"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["RabbitMqPort"]),
                UserName = ConfigurationManager.AppSettings["RabbitMQUserName"],
                Password = ConfigurationManager.AppSettings["RabbitMQPassword"],
            };

            connection = connectionFactory.CreateConnection();
        }

        public static Consumer Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void StartConsumer()
        {
            var connection = this.connection;
                var channel = connection.CreateModel();

                channel.QueueDeclare(queue: ConfigurationManager.AppSettings["RabbitMQQueueName"],
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Debug.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: ConfigurationManager.AppSettings["RabbitMQQueueName"],
                                     autoAck: true,
                                     consumer: consumer);

        }
    }
}
