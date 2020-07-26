using CMS.PublisherAPI.Models;
using CMS.RabbitMQ.Core;
using CMS.RabbitMQ.Core.Enums;
using CMS.RabbitMQ.Core.Implementations;
using CMS.RabbitMQ.Core.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace CMS.PublisherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        [HttpPost]
        public void Add(Student student)
        {
            IPublisher p = new Publisher();
            var payload = new RabbitMQPayLoad();
            payload.MessageBody = student;

            //p.PublishInRabbitMQQueue(payload "student-queue");
            student.Address = "send via  exchage to student queue";
            p.PublishInRabbitMQExchange(Queues.CMS_Student.ToString().ToLower(), payload,
                Exchange.CMS_Exchange.ToString().ToLower(), ExchangeType.Direct,
                StudentConsumerEvents.Student_Login.ToString().ToLower());
        }
    }
}