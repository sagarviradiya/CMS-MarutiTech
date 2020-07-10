using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Consumer.Services
{
    public abstract class DefaultConsumer : IConsumer
    {
        /// <summary>
        /// initialize consumer
        /// </summary>
        public abstract void Init();

        protected virtual void ConsumeFromQueue(string queueName)
        {
            try
            {
                RabbitMqConnectionService.SingleInstance.CheckConnection();
                var connection = RabbitMqConnectionService.SingleInstance.Connection;
                var channel = RabbitMqConnectionService.SingleInstance.Channel;
                channel.BasicQos(0, 1, false);
                channel.QueueDeclare(queueName, true, false, false, null);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_ReceivedFromQueue;
                channel.BasicConsume(queueName, false, consumer);

            }
            catch (Exception)
            {
                throw;
            }
        }
        protected virtual void ConsumeFromExchange(string queueName, string exchangeName, List<string> routeKeys)
        {
            try
            {
                RabbitMqConnectionService.SingleInstance.CheckConnection();
                var connection = RabbitMqConnectionService.SingleInstance.Connection;
                var channel = RabbitMqConnectionService.SingleInstance.Channel;
                channel.BasicQos(0, 1, false);
                channel.QueueDeclare(queueName, true, false, false, null);
                foreach (string routingKey in routeKeys)
                {
                    channel.QueueBind(queueName, exchangeName, routingKey);
                }
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_ReceivedFromExchange;
                channel.BasicConsume(queueName, false, consumer);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Consumer_ReceivedFromExchange(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            Debug.WriteLine("Message Recieved From Exchange" + message);
        }

        private void Consumer_ReceivedFromQueue(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            Debug.WriteLine("Message Recieved" + message);
        }
    }
}
