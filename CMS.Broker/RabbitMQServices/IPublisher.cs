using CMS.Broker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Broker.RabbitMQServices
{
    public interface IPublisher
    {
        void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)  where T : class;
    }

}
