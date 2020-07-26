using System;
using System.Collections;
using System.Text;
using CMS.RabbitMQ.Core.Models;
using CMS.RabbitMQ.Core.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CMS.RabbitMQ.Core.Implementations
{
    public abstract class DefaultPublisher : IPublisher
    {
        /// <summary>
        ///     Publish message to Exchange
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message">type of message</param>
        /// <param name="exchangeName">exchange to route message to queue</param>
        /// <param name="exchangeType">exchange type  direct,topic, fanout,header</param>
        /// <param name="routeKey">based on key , exchange will route message to queue</param>
        public virtual void PublishInRabbitMQExchange(string queueName, RabbitMQPayLoad message, string exchangeName,
            string exchangeType, string routeKey)
        {
            try
            {
                if (message?.MessageBody == null)
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
                        var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
                        channel.BasicPublish(exchangeName.ToLower(), routeKey.ToLower(), null, messageBytes);
                    }
                }
                else
                {
                    var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message.MessageBody));
                    channel.BasicPublish(exchangeName.ToLower(), routeKey.ToLower(), properties, messageBytes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Publish message to Queue
        /// </summary>
        /// <typeparam name="T">class</typeparam>
        /// <param name="message">object of message</param>
        /// <param name="queueName">queue name directly</param>
        public virtual void PublishInRabbitMQQueue(RabbitMQPayLoad message, string queueName)
        {
            try
            {
                if (message == null || message.MessageBody == null)
                    return;

                var rabbitConnectionService = RabbitMqConnectionService.SingleInstance;

                var connection = rabbitConnectionService.Connection;
                var channel = rabbitConnectionService.Channel;
                channel.QueueDeclare(queueName, true, false, false, null);

                channel.BasicQos(0, 1, false);
                if (message.MessageBody is IEnumerable messages)
                {
                    foreach (var item in messages)
                    {
                        var dataObj = JsonConvert.SerializeObject(item);
                        var messageBytes = Encoding.UTF8.GetBytes(dataObj);
                        var properties = channel.CreateBasicProperties();

                        properties.Persistent = true;
                        channel.BasicQos(0, 1, false);
                        channel.BasicPublish(string.Empty, queueName, properties, messageBytes);
                    }
                }
                else
                {
                    var dataObj = JsonConvert.SerializeObject(message.MessageBody);
                    var messageBytes = Encoding.UTF8.GetBytes(dataObj);
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