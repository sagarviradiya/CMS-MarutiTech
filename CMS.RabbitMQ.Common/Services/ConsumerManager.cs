using CMS.RabbitMQ.Common.Consumers;
using CMS.RabbitMQ.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.RabbitMQ.Common.Services
{
    public class ConsumerManager
    {
        private static readonly Lazy<ConsumerManager> _singleton = new Lazy<ConsumerManager>(() => new ConsumerManager());
        private ConsumerManager() { }

        public static ConsumerManager Instance
        {
            get
            {
                return _singleton.Value;
            }
        }

        public void Init()
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
