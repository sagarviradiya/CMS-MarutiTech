using CMS.Consumer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Consumer.Consumers
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

        public override void Init()
        {
            ConsumeFromQueue("student-queue");
        }
    }
}
