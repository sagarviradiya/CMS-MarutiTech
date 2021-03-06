﻿using CMS.RabbitMQ.Common.Enums;
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
        private readonly string hostName;
        private readonly int port;
        private readonly string userName;
        private readonly string password;
        private static ConnectionFactory connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private static object _connectionLockObj = new object();

        private RabbitMqConnectionService()
        {
        }
        public static RabbitMqConnectionService SingleInstance => _instance.Value;

        public IConnection Connection { get => _connection; private set => _connection = value; }
        public IModel Channel { get => _channel; private set => _channel = value; }

        private void RetryConnection()
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
        private void Connection_Shutdown(object sender, ShutdownEventArgs e)
        {
            RetryConnection();
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


        public void CheckConnection(string hostName, int port, string userName, string password)
        {
            if (_connection != null && _connection.IsOpen)
            {
                return;
            }
            connectionFactory = new ConnectionFactory()
            {
                HostName =hostName,
                Port = port,
                UserName = userName,
                Password = password ,
                RequestedHeartbeat = TimeSpan.FromSeconds(30),
            };
            lock (_connectionLockObj)
            {
                try
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        _connection = connectionFactory.CreateConnection();
                        _channel = _connection.CreateModel();
                        _connection.ConnectionShutdown += Connection_Shutdown;
                    }
                }
                catch
                {
                    ;
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
            CheckConnection();

            var routekeys = Enum.GetNames(typeof(StudentConsumerEvents)).ToList() ?? new List<string>();

            _channel.BasicQos(0, 1, false);
            foreach (var queue in Enum.GetNames(typeof(Queues)))
            {
                _channel.ExchangeDeclare(Exchange.CMS_Exchange.ToString().ToLower(), ExchangeType.Direct, true, false, null);
                _channel.QueueDeclare(queue.ToLower(), true, false, false, null);
            }
            var studentConsumerEvents = EnumUtility.BindQueueToStudentEvents();

            foreach (var routeKey in studentConsumerEvents.Value)
            {
                _channel.QueueBind(studentConsumerEvents.Key.ToLower(), Exchange.CMS_Exchange.ToString().ToLower(), routeKey.ToLower());
            }

        }

    }
}