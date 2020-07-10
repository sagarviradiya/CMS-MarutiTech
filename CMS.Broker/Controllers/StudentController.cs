using CMS.Broker.Models;
using CMS.Broker.RabbitMQServices;
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
            p.PublishInRabbitMQExchange("student-queue",payload, "exchange", ExchangeType.Direct, "#.student.#");
            student.Address = "send via  exchage to college queue";

            p.PublishInRabbitMQExchange("college-queue",payload, "exchange", ExchangeType.Direct, "#.college.#");

        }

    }
}