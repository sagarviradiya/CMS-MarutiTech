using System;
using System.Diagnostics;
using System.Text;
using CMS.RabbitMQ.Core.Enums;
using CMS.RabbitMQ.Core.Implementations;
using CMS.RabbitMQ.Core.Interfaces;
using CMS.RabbitMQ.Core.Models;
using CMS.RabbitMQ.Core.Services;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace CMS.RabbitMQ.Core.Consumers
{
    public class StudentConsumer : DefaultConsumer
    {
        public IConsumerDataProcessor ConsumerDataProcessor { get; set; }

        public StudentConsumer()
        {
        }


        protected override void Consumer_ReceivedFromExchange(object sender, BasicDeliverEventArgs e)
        {
            var payload = new RabbitMQPayLoad
            {
                EventTime = DateTime.UtcNow,
                MessageBody = Encoding.UTF8.GetString(e.Body.ToArray()),
                RoutingKey = e.RoutingKey
            };
            //ConsumerDataProcessor.ProcessResponse(payload);
        }

        protected override void Consumer_ReceivedFromQueue(object sender, BasicDeliverEventArgs e)
        {
        }
    }
}