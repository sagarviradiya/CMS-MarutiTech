using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS.RabbitMQ.Core.Enums
{
    public class EnumUtility
    {
        public static KeyValuePair<string, List<string>> BindQueueToStudentEvents()
        {
            var events = Enum.GetNames(typeof(StudentConsumerEvents)).ToList();
            return new KeyValuePair<string, List<string>>(Queues.CMS_Student.ToString(), events);
        }

        public static KeyValuePair<string, List<string>> BindQueueToCollegeEvents()
        {
            var events = Enum.GetNames(typeof(CollegeConsumerEvents)).ToList();
            return new KeyValuePair<string, List<string>>(Queues.CMS_College.ToString(), events);
        }
    }
}