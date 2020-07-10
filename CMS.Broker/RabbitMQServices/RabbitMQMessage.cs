using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Broker.RabbitMQServices
{
    public class RabbitMQMessage
    {
        public string MessageBody { get; set; }
        public string EventType { get; set; }
    }
}