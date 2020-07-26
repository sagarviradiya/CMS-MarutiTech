namespace CMS.RabbitMQ.Core
{
    internal interface IConsumer
    {
        void Init();
        void ConsumeFromQueue(string queueName);
        void ConsumeFromExchange(string queueName,string exchangeName);
    }
}