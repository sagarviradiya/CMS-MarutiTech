using CMS.Broker.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace CMS.Broker.RabbitMQServices
{
    public class Publisher : IPublisher
    {
        public void PublishStudentCreated(Student student)
        {

            using (var connection = RabbitMqConnectionService.SingleInstance.GetRabbitMQConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: ConfigurationManager.AppSettings["RabbitMQQueueName"],
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                string message = JsonConvert.SerializeObject(student);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                        routingKey: ConfigurationManager.AppSettings["RabbitMQQueueName"],
                                        basicProperties: null,
                                        body: body);
            }
        }
    }
}