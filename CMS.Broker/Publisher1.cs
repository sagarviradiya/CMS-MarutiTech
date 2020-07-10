
//using RabbitMQ.Client;
//using System;
//using System.Collections.Generic;

//public class Publisher
//{

//    private IConnection _connection;

//    private static object LockObject = new object();

//    private static Publisher Instance;

//    private Publisher()
//    {
//    }

//    public static Publisher GetInstance()
//    {
//        if (Instance == null)
//        {
//            lock (LockObject)
//            {
//                if (Instance == null)
//                {
//                    Instance = new Publisher();
//                }
//            }
//        }
//        return Instance;
//    }

//    private ConnectionFactory GetConnectionFactory(string hostName, int portNumber)
//    {
//        return new ConnectionFactory
//        {
//            HostName = hostName,
//            Port = portNumber
//        };
//    }

//    private void GetConnection(string hostName, int portNumber)
//    {
//        if (_connection != null && _connection.IsOpen)
//        {
//            return;
//        }
//        lock (LockObject)
//        {
//            if (_connection == null || !_connection.IsOpen)
//            {
//                try
//                {
//                    ConnectionFactory factory = GetConnectionFactory(hostName, portNumber);
//                    _connection = factory.CreateConnection();
//                }
//                catch (Exception exception)
//                {
//                }
//            }
//        }
//    }


//    public void PublishMessagesInExchange(string hostName, int portNumber, string exchange, List<RabbitMQMessage> messages)
//    {
//        try
//        {
//            GetConnection(hostName, portNumber);
//            using (IModel channel = _connection.CreateModel())
//            {
//                channel.ExchangeDeclare(exchange, "direct", durable: true, autoDelete: false, null);
//                foreach (RabbitMQMessage message in messages)
//                {
//                    string routingKey = message.EventType.ToLower();
//                    string serializesMessage = Utility.SerializeObjectToXml(message);
//                    byte[] messageBytes = Encoding.UTF8.GetBytes(serializesMessage);
//                    channel.BasicPublish(exchange, routingKey, null, messageBytes);
//                    Log.LogDebug($"Record added into queue : \n {serializesMessage}: ");
//                }
//            }
//        }
//        catch (Exception exception)
//        {
//            string data = $"Exchange: {exchange}, Host: {hostName}, Port: {portNumber}. \n Messages: {Utility.SerializeObjectToXml(messages)}";
//            Log.LogException(exception, data, "Failed to publish messages in RabbitMQ exchange. Error: " + exception.Message, ShouldSendEmail: true);
//        }
//    }

//    public void PublishMessagesInQueue(string hostName, int portNumber, string queue, List<object> messages)
//    {
//        try
//        {
//            GetConnection(hostName, portNumber);
//            using (IModel channel = _connection.CreateModel())
//            {
//                channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, null);
//                foreach (object message in messages)
//                {
//                    string serializesMessage = Utility.SerializeSoapObject(message);
//                    byte[] messageBytes = Encoding.UTF8.GetBytes(serializesMessage);
//                    channel.BasicPublish("", queue, null, messageBytes);
//                    Log.LogDebug($"Record added into queue : \n {serializesMessage}: ");
//                }
//            }
//        }
//        catch (Exception exception)
//        {
//            string data = $"Queue: {queue}, Host: {hostName}, Port: {portNumber}. \n Messages: {Utility.SerializeObjectToXml(messages)}";
//            Log.LogException(exception, data, "Failed to publish messages in RabbitMQ exchange. Error: " + exception.Message, ShouldSendEmail: true);
//        }
//    }

//    public void PublishMessagesInRetryExchange(string hostName, int portNumber, string exchange, string retryExchange, string queue, List<RabbitMQMessage> messages, string timeout)
//    {
//        try
//        {
//            GetConnection(hostName, portNumber);
//            using (IModel channel = _connection.CreateModel())
//            {
//                Dictionary<string, object> queueArgs = new Dictionary<string, object>
//                {
//                    {
//                        "x-dead-letter-exchange",
//                        exchange
//                    }
//                };
//                channel.ExchangeDeclare(retryExchange, "direct", durable: true, autoDelete: false, null);
//                channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, queueArgs);
//                foreach (RabbitMQMessage message in messages)
//                {
//                    string serializesMessage = Utility.SerializeSoapObject(message);
//                    string routingKey = message.EventType.ToLower();
//                    channel.QueueBind(queue, retryExchange, routingKey, null);
//                    byte[] messageBytes = Encoding.UTF8.GetBytes(serializesMessage);
//                    IBasicProperties properties = channel.CreateBasicProperties();
//                    properties.Expiration = timeout;
//                    channel.BasicPublish(retryExchange, routingKey, properties, messageBytes);
//                    Log.LogDebug($"Record added into queue : \n {serializesMessage}: ");
//                }
//            }
//        }
//        catch (Exception exception)
//        {
//            _ = $"Queue: {queue}, Host: {hostName}, Port: {portNumber}. \n Messages: {Utility.SerializeObjectToXml(messages)}";
//            Log.LogException(exception, "", "Failed to publish messages in RabbitMQ exchange. Error: " + exception.Message, ShouldSendEmail: true);
//        }
//    }
//}
//public class RabbitMQMessage
//{
//    public string EventType
//    {
//        get;
//        set;
//    }

//    public string EventBody
//    {
//        get;
//        set;
//    }

//    public DateTime? EventTime
//    {
//        get;
//        set;
//    }
//}