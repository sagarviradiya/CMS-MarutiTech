using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.RabbitMQ.Common.Models
{
    public class RabbitMQPayLoad
    {
        public RabbitMQPayLoad()
        {

        }
        public object MessageBody { get; set; }

        public string RoutingKey { get; set; }
        public DateTime EventTime { get; set; }
    }
}