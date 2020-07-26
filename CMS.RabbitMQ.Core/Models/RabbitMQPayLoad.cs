using System;

namespace CMS.RabbitMQ.Core.Models
{
    public class RabbitMQPayLoad
    {
        public object MessageBody { get; set; }

        public string RoutingKey { get; set; }
        public DateTime EventTime { get; set; }
    }
}