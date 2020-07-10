using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CMS.Consumer.Services
{
    public sealed class RabbitMqConnectionService
    {

        private static readonly Lazy<RabbitMqConnectionService> _instance = new Lazy<RabbitMqConnectionService>(() => new RabbitMqConnectionService());
        private readonly IConnection _connection;

        private RabbitMqConnectionService() => _connection = GetRabbitMQConnection();



        public static RabbitMqConnectionService SingleInstance => _instance.Value;
       
        public IConnection GetRabbitMQConnection()
        {

            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = ConfigurationManager.AppSettings["RabbitMQHostName"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["RabbitMqPort"]),
                UserName = ConfigurationManager.AppSettings["RabbitMQUserName"],
                Password = ConfigurationManager.AppSettings["RabbitMQPassword"],
            };

            return connectionFactory.CreateConnection();
        }
        public IModel GetModel()
        {
            return _connection.CreateModel();
        }
        

    }
}