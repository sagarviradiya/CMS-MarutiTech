using CMS.RabbitMQ.Common.Enums;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace CMS.RabbitMQ.Common.Services
{
    public sealed class RabbitMqConnectionService
    {

        private static readonly Lazy<RabbitMqConnectionService> _instance = new Lazy<RabbitMqConnectionService>(() => new RabbitMqConnectionService());
        private static ConnectionFactory factory;
        private IConnection _connection;
        private IModel _channel;
        private static object _connectionLockObj = new object();

        private RabbitMqConnectionService()
        {
            CheckConnection();
        }
        public static RabbitMqConnectionService SingleInstance => _instance.Value;

        public IConnection Connection { get => _connection; private set => _connection = value; }
        public IModel Channel { get => _channel; private set => _channel = value; }

        private void ReconnectWithTry()
        {
            Close();

            var mres = new ManualResetEventSlim(false);

            while (!mres.Wait(3000))
            {
                try
                {
                    CheckConnection();
                    Debug.WriteLine("Reconnected!");
                    mres.Set();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Reconnect failed!");
                }
            }
        }
        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            ReconnectWithTry();
        }

        private void Close()
        {
            try
            {
                if (_channel != null && _channel.IsOpen)
                {
                    _channel.Close();
                    _channel = null;
                }

                if (_connection != null && _connection.IsOpen)
                {
                    _connection.Close();
                    _connection = null;
                }
            }
            catch (IOException ex)
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
        }


        public void CheckConnection()
        {
            if (_connection != null && _connection.IsOpen)
            {
                return;
            }
            factory = new ConnectionFactory()
            {
                HostName = ConfigurationManager.AppSettings["RabbitMQHostName"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["RabbitMqPort"]),
                UserName = ConfigurationManager.AppSettings["RabbitMQUserName"],
                Password = ConfigurationManager.AppSettings["RabbitMQPassword"],
                RequestedHeartbeat = TimeSpan.FromSeconds(30),
            };
            lock (_connectionLockObj)
            {
                try
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        _connection = factory.CreateConnection();
                        _channel = _connection.CreateModel();
                        _connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public IModel GetModel()
        {
            return _connection.CreateModel();
        }
        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
                return true;
            else
            {
                obj?.Dispose();
                return false;
            }
        }
        public void Init()
        {
            RabbitMqConnectionService.SingleInstance.CheckConnection();
            var connection = RabbitMqConnectionService.SingleInstance.Connection;
            var channel = RabbitMqConnectionService.SingleInstance.Channel;
            var routekeys = Enum.GetNames(typeof(StudentConsumerEvents)).ToList() ?? new List<string>();

            channel.BasicQos(0, 1, false);
            foreach (var queue in Enum.GetNames(typeof(Queues)))
            {
                channel.ExchangeDeclare(Exchange.CMS_Exchange.ToString().ToLower(), ExchangeType.Direct, true, false, null);
                channel.QueueDeclare(queue.ToLower(), true, false, false, null);
            }
            var studentConsumerEvents = EnumUtility.BindQueueToStudentEvents();

            foreach (var routeKey in studentConsumerEvents.Value)
            {
                channel.QueueBind(studentConsumerEvents.Key.ToLower(), Exchange.CMS_Exchange.ToString().ToLower(), routeKey.ToLower());
            }

        }

    }
}