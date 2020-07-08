using CMS.Broker.Models;
using CMS.Broker.RabbitMQServices;
using System.Web.Http;

namespace CMS.Broker.Controllers
{
    public class StudentController : ApiController
    {

        [HttpPost]
        public void Add(Student student)
        {
            IPublisher p = new Publisher();
            p.PublishStudentCreated(student);
        }

    }
}