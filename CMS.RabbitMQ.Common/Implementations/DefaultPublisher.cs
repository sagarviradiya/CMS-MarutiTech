using CMS.RabbitMQ.Common.Enums;
using CMS.RabbitMQ.Common.Models;
using CMS.RabbitMQ.Common.Publisher;
using CMS.RabbitMQ.Common.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.RabbitMQ.Common.Implementations
{
    public abstract class DefaultPublisher : IPublisher
    {

        /// <summary>
        /// Publish message to Exchange
        /// </summary>
        /// <typeparam name="T">class</typeparam>
        /// <param name="message">type of message</param>
        /// <param name="exchangeName">exchange to route message to queue</param>
        /// <param name="exchangeType">exchange type  direct,topic, fanout,header</param>
        /// <param name="routeKey">based on key , exchange will route message to queue</param>
        public void PublishInRabbitMQExchange(string queueName, RabbitMQPayLoad message, string exchangeName, string exchangeType, string routeKey)
        {
            try
            {
                if (message == null || message.MessageBody == null)
                    return;


                var rabbitConnectionService = RabbitMqConnectionService.SingleInstance;
                rabbitConnectionService.Init();

                var channel = rabbitConnectionService.Channel;
                var properties = channel.CreateBasicProperties();

                properties.Persistent = true;
                channel.BasicQos(0, 1, false);

                if (message.MessageBody is IEnumerable messages)
                {
                    foreach (var item in messages)
                    {
                        byte[] messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
                        channel.BasicPublish(exchangeName.ToLower(), routeKey.ToLower(), null, messageBytes);
                    }
                }
                else
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message.MessageBody));
                    channel.BasicPublish(exchangeName.ToLower(), routeKey.ToLower(), properties, messageBytes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Publish message to Queue
        /// </summary>
        /// <typeparam name="T">class</typeparam>
        /// <param name="message">object of message</param>
        /// <param name="queueName">queue name directly</param>
        public void PublishInRabbitMQQueue(RabbitMQPayLoad message, string queueName)
        {
            try
            {
                if (message == null || message.MessageBody == null)
                    return;

                var rabbitConnectionService = RabbitMqConnectionService.SingleInstance;

                var connection = rabbitConnectionService.Connection;
                var channel = rabbitConnectionService.Channel;
                channel.QueueDeclare(queueName, true, exclusive: false, autoDelete: false, arguments: null);

                channel.BasicQos(0, 1, false);
                if (message.MessageBody is IEnumerable messages)
                {
                    foreach (var item in messages)
                    {
                        var dataObj = JsonConvert.SerializeObject(item);
                        byte[] messageBytes = Encoding.UTF8.GetBytes(dataObj);
                        var properties = channel.CreateBasicProperties();

                        properties.Persistent = true;
                        channel.BasicQos(0, 1, false);
                        channel.BasicPublish(string.Empty, queueName, properties, messageBytes);
                    }
                }
                else
                {
                    var dataObj = JsonConvert.SerializeObject(message.MessageBody);
                    byte[] messageBytes = Encoding.UTF8.GetBytes(dataObj);
                    var properties = channel.CreateBasicProperties();

                    properties.Persistent = true;
                    channel.BasicQos(0, 1, false);
                    channel.BasicPublish(string.Empty, queueName, properties, messageBytes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
