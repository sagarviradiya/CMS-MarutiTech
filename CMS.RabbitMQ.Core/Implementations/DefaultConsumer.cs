using CMS.RabbitMQ.Core.Enums;
using CMS.RabbitMQ.Core.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CMS.RabbitMQ.Core.Implementations
{
    public abstract class DefaultConsumer : IConsumer
    {
        private RabbitMqConnectionService RabbitMqService
        {
            get => RabbitMqConnectionService.SingleInstance;
        }

        /// <summary>
        ///     initialize consumer
        /// </summary>
        public virtual void Init()
        {
            RabbitMqService.Init();
            ConsumeFromQueue(Queues.CMS_Student.ToString());
            ConsumeFromExchange(Queues.CMS_Student.ToString(),Exchange.CMS_Exchange.ToString());
        }

        public virtual void ConsumeFromQueue(string queueName)
        {
            RabbitMqService.CheckConnection();
            var channel = RabbitMqService.Channel;
            channel.BasicQos(0, 1, false);
            channel.QueueDeclare(queueName, true, false, false, null);
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queueName, false, consumer);
        }

        public virtual void ConsumeFromExchange(string queueName, string exchangeName)
        {
            var consumer = new EventingBasicConsumer(RabbitMqService.Channel);
            consumer.Received += Consumer_ReceivedFromExchange;
            RabbitMqService.Channel.BasicConsume(queueName.ToLower(), false, consumer);
        }

        protected abstract void Consumer_ReceivedFromExchange(object sender, BasicDeliverEventArgs e);
        protected abstract void Consumer_ReceivedFromQueue(object sender, BasicDeliverEventArgs e);
    }
}