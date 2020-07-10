using CMS.Consumer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Consumer.Consumers
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
            ConsumeFromQueue("college-queue");
        }
    }
}
