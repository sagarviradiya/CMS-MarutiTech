using CMS.Broker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Broker.RabbitMQServices
{
    public interface IPublisher
    {
        void PublishInRabbitMQExchange(string queueName, RabbitMQPayLoad message, string exchangeName, string exchangeType, string routeKey);
        void PublishInRabbitMQQueue(RabbitMQPayLoad message, string queueName);
    }

}
