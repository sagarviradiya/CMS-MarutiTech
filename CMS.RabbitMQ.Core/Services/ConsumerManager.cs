using System;
using System.Collections.Generic;
using System.Threading;
using CMS.RabbitMQ.Core.Consumers;

namespace CMS.RabbitMQ.Core.Services
{
    public class ConsumerManager : IConsumerManager
    {
        private static readonly Lazy<ConsumerManager> _singleton =
            new Lazy<ConsumerManager>(() => new ConsumerManager());

        private ConsumerManager()
        {
        }

        public static ConsumerManager Instance => _singleton.Value;

        public void Init()
        {
            
            var consumers = new List<IConsumer> {new StudentConsumer(), new CollegeConsumer()};

            foreach (var consumer in consumers)
            {
                var thread = new Thread(consumer.Init);
                thread.IsBackground = true;
                thread.Start();
            }
        }
    }
}