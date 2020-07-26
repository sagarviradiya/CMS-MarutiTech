using System.Diagnostics;
using System.Text;
using CMS.Core.Services;
using CMS.Data;
using CMS.Data.DataServices;
using CMS.RabbitMQ.Core.Interfaces;
using CMS.RabbitMQ.Core.Models;

namespace CMS.RabbitMQ.DataAccess
{
    public class StudentDataConsumer : IConsumerDataProcessor
    {
        private readonly IStudentService _studentService;

        public StudentDataConsumer(StudentDBService dbService)
        {
            this._studentService = dbService;
        }
        
        public void ProcessResponse(RabbitMQPayLoad payLoad)
        {
            var message = payLoad.MessageBody;
            Debug.WriteLine("Message Recieved From Exchange" + message.ToString());
        }
    }
}