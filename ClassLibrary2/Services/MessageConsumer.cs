using ClassLibrary2.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ClassLibrary2.Consumer.Services
{
    public class MessageConsumer
    {

        public static void StartAllConsumer()
        {
            try
            {
                Thread thread = new Thread(new ThreadStart(ClassLibrary2.Services.Consumer.Instance.StartConsumer));
                thread.IsBackground = true;
                thread.Start();

            }
            catch (Exception exception)
            {
            }
        }
    }
}