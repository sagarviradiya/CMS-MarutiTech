using CMS.RabbitMQ.Common.Enums;
using CMS.RabbitMQ.Common.Implementations;
using System;

namespace CMS.RabbitMQ.Common.Consumers
{
    public class StudentConsumer : DefaultConsumer
    {
        private static readonly Lazy<StudentConsumer> _singleton = new Lazy<StudentConsumer>(() => new StudentConsumer());
        private StudentConsumer() { }

        public static StudentConsumer Instance
        {
            get
            {
                return _singleton.Value;
            }
        }

        public  override void Init()
        {
            ConsumeFromExchange(Queues.CMS_Student.ToString(), Exchange.CMS_Exchange.ToString());
        }
    }
}
