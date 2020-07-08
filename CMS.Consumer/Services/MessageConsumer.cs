using System;
using System.Threading;

namespace CMS.Consumer.Services
{
    public class MessageConsumer
    {

        public static void StartAllConsumer()
        {
            try
            {
                Thread thread = new Thread(new ThreadStart(Consumer.Instance.StartConsumer));
                thread.IsBackground = true;
                thread.Start();

            }
            catch (Exception exception)
            {
            }
        }
    }
}