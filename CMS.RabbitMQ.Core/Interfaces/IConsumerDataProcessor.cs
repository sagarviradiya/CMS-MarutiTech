using CMS.RabbitMQ.Core.Models;

namespace CMS.RabbitMQ.Core.Interfaces
{
    public interface IConsumerDataProcessor
    {
        void ProcessResponse(RabbitMQPayLoad payLoad);
    }
}