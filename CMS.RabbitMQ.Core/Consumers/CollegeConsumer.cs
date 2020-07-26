using System;
using System.Diagnostics;
using System.Text;
using CMS.RabbitMQ.Core.Implementations;
using CMS.RabbitMQ.Core.Interfaces;
using CMS.RabbitMQ.Core.Services;
using RabbitMQ.Client.Events;

namespace CMS.RabbitMQ.Core.Consumers
{
    public class CollegeConsumer : DefaultConsumer
    {
        public IConsumerDataProcessor ConsumerDataProcessor { get; set; }

        protected override void Consumer_ReceivedFromExchange(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Debug.WriteLine("Message Recieved From Exchange" + message);
        }

        protected override void Consumer_ReceivedFromQueue(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Debug.WriteLine("Message Recieved From Exchange" + message);
        }
    }
}