using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace CMS.Broker.RabbitMQServices
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
            Connect();
            _connection.ConnectionShutdown -= Connection_ConnectionShutdown;
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
                    Connect();
                    Console.WriteLine("Reconnected!");
                    mres.Set();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Reconnect failed!");
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


        public void Connect()
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
                        _connection.ConnectionShutdown += Connection_ConnectionShutdown;
                        _channel = GetModel();
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

    }
}