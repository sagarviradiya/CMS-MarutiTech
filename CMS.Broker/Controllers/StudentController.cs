using CMS.Broker.Models;
using CMS.RabbitMQ.Common.Enums;
using CMS.RabbitMQ.Common.Models;
using CMS.RabbitMQ.Common.Publisher;
using RabbitMQ.Client;
using System.Web.Http;

namespace CMS.Broker.Controllers
{
    public class StudentController : ApiController
    {

        [HttpPost]
        public void Add(Student student)
        {
            IPublisher p = new Publisher();
            var payload = new RabbitMQPayLoad();
            payload.MessageBody = student;

            //p.PublishInRabbitMQQueue(payload "student-queue");
            student.Address = "send via  exchage to student queue";
            p.PublishInRabbitMQExchange(Queues.CMS_Student.ToString().ToLower(), payload, Exchange.CMS_Exchange.ToString().ToLower(), ExchangeType.Direct, StudentConsumerEvents.Student_Login.ToString().ToLower());
            

        }

    }
}