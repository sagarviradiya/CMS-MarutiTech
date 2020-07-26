using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CMS.RabbitMQ.Core.Enums;
using CMS.RabbitMQ.Core.Models;
using RabbitMQ.Client;

namespace CMS.RabbitMQ.Core.Services
{
    public sealed class RabbitMqConnectionService : IRabbitMqConnectionService
    {
        private static readonly Lazy<RabbitMqConnectionService> _instance =
            new Lazy<RabbitMqConnectionService>(() => new RabbitMqConnectionService());

        private static ConnectionFactory connectionFactory;
        private static readonly object _connectionLockObj = new object();
        private readonly RabbitMQConnectionData rabbitMQConnectionData;

        private RabbitMqConnectionService()
        {
            rabbitMQConnectionData = new RabbitMQConnectionData();
            GetConnectionParameters();
            CheckConnection();
        }

        public static RabbitMqConnectionService SingleInstance => _instance.Value;

        public IConnection Connection { get; private set; }

        public IModel Channel { get; set; }

        private void GetConnectionParameters()
        {
            rabbitMQConnectionData.Host = ConfigurationService.Get("RabbitMQ:Host");
            rabbitMQConnectionData.Port = Convert.ToInt32(ConfigurationService.Get("RabbitMQ:Port"));
            rabbitMQConnectionData.UserName = ConfigurationService.Get("RabbitMQ:UserName");
            rabbitMQConnectionData.Password = ConfigurationService.Get("RabbitMQ:Password");
        }

        private void RetryConnection()
        {
            Close();

            var mres = new ManualResetEventSlim(false);

            while (!mres.Wait(3000))
                try
                {
                    CheckConnection();
                    Debug.WriteLine("Reconnected!");
                    mres.Set();
                }
                catch
                {
                    Debug.WriteLine("Reconnect failed!");
                }
        }

        private void Connection_Shutdown(object sender, ShutdownEventArgs e)
        {
            RetryConnection();
        }

        private void Close()
        {
            try
            {
                if (Channel != null && Channel.IsOpen)
                {
                    Channel.Close();
                    Channel = null;
                }

                if (Connection != null && Connection.IsOpen)
                {
                    Connection.Close();
                    Connection = null;
                }
            }
            catch (IOException)
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
        }


        public void CheckConnection()
        {
            if (Connection != null && Connection.IsOpen) return;

            lock (_connectionLockObj)
            {
                connectionFactory = new ConnectionFactory
                {
                    HostName = rabbitMQConnectionData.Host,
                    Port = rabbitMQConnectionData.Port,
                    UserName = rabbitMQConnectionData.UserName,
                    Password = rabbitMQConnectionData.Password,
                    RequestedHeartbeat = TimeSpan.FromSeconds(30)
                };
                if (Connection == null || !Connection.IsOpen)
                {
                    Connection = connectionFactory.CreateConnection();
                    Channel = Connection.CreateModel();
                    Connection.ConnectionShutdown += Connection_Shutdown;
                }
            }
        }

        public void Init()
        {
            CheckConnection();

            Channel.BasicQos(0, 1, false);
            foreach (var queue in Enum.GetNames(typeof(Queues)))
            {
                Channel.ExchangeDeclare(Exchange.CMS_Exchange.ToString().ToLower(), ExchangeType.Direct, true, false,
                    null);
                Channel.QueueDeclare(queue.ToLower(), true, false, false, null);
            }

            var studentConsumerEvents = EnumUtility.BindQueueToStudentEvents();

            foreach (var routeKey in studentConsumerEvents.Value)
                Channel.QueueBind(studentConsumerEvents.Key.ToLower(), Exchange.CMS_Exchange.ToString().ToLower(),
                    routeKey.ToLower());
        }
    }
}