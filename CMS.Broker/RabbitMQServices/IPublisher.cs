using CMS.Broker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Broker.RabbitMQServices
{
    public interface IPublisher
    {
        void PublishStudentCreated(Student student);
    }
}