using CMS.Consumer.Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.Consumer.Services
{
    public class RabbitMQConsumerManager
    {
        private static readonly Lazy<RabbitMQConsumerManager> _singleton = new Lazy<RabbitMQConsumerManager>(() => new RabbitMQConsumerManager());
        private RabbitMQConsumerManager() { }

        public static RabbitMQConsumerManager Instance
        {
            get
            {
                return _singleton.Value;
            }
        }

        public  void Init()
        {
            List<IConsumer> consumers = new List<IConsumer>();
            consumers.Add(StudentConsumer.Instance);   
            consumers.Add(CollegeConsumer.Instance);
        
            foreach (IConsumer consumer in consumers)
            {
                Thread thread = new Thread(new ThreadStart(consumer.Init));
                thread.IsBackground = true;
                thread.Start();
            }
        }
    }
}
