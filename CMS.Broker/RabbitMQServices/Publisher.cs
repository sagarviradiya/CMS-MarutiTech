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
        

        public Publisher()
        {
           
        }


        public void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey)
            where T : class
        {
            var rabbitConnectionService = RabbitMqConnectionService.SingleInstance;
            if (message == null)
                return;
            var channel = rabbitConnectionService.GetModel();
            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, true, false, null);

                var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                var properties = channel.CreateBasicProperties();

                properties.Persistent = true;

                channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rabbitConnectionService.Return(channel);
            }
        }
        //public void PublishStudentCreated(Student student)
        //{

        //    using (var connection = RabbitMqConnectionService.SingleInstance.GetRabbitMQConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.QueueDeclare(queue: ConfigurationManager.AppSettings["RabbitMQQueueName"],
        //                                durable: true,
        //                                exclusive: false,
        //                                autoDelete: false,
        //                                arguments: null);
        //        channel.BasicQos(0, 1, false);

        //        string message = JsonConvert.SerializeObject(student);
        //        var body = Encoding.UTF8.GetBytes(message);

        //        channel.BasicPublish(exchange: "",
        //                                routingKey: ConfigurationManager.AppSettings["RabbitMQQueueName"],
        //                                basicProperties: null,
        //                                body: body);
        //    }
        //}
    }
}