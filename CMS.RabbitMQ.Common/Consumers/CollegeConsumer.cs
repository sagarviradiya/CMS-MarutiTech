using CMS.RabbitMQ.Common.Enums;
using CMS.RabbitMQ.Common.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.RabbitMQ.Common.Consumers
{
    public class CollegeConsumer : DefaultConsumer
    {
        private static readonly Lazy<CollegeConsumer> _singleton = new Lazy<CollegeConsumer>(() => new CollegeConsumer());
        private CollegeConsumer() { }

        public static CollegeConsumer Instance
        {
            get
            {
                return _singleton.Value;
            }
        }

        public override void Init()
        {
            ConsumeFromExchange(Queues.CMS_College.ToString(), Exchange.CMS_Exchange.ToString());
        }
    }
}
