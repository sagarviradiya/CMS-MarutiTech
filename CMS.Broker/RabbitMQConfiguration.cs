using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Broker
{
    public class RabbitMQConfiguration
    {

        public string HostName { get; set; }

        public string QueueName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public string Port { get; set; }

    }
}