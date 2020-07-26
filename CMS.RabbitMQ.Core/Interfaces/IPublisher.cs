using CMS.RabbitMQ.Core.Models;

namespace CMS.RabbitMQ.Core
{
    public interface IPublisher
    {
        void PublishInRabbitMQExchange(string queueName, RabbitMQPayLoad message, string exchangeName,
            string exchangeType, string routeKey);

        void PublishInRabbitMQQueue(RabbitMQPayLoad message, string queueName);
    }
}