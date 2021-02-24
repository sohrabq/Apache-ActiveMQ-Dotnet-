using Apache.NMS;
using Microsoft.Extensions.Logging;
using System;

namespace ConsumerTwo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("consumer two connected");
            var logFactory = new LoggerFactory();

            var logger = logFactory.CreateLogger<Program>();
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");
            logger.LogInformation($"about to connect to {connecturi}");
            var factory = new Apache.NMS.ActiveMQ.ConnectionFactory(connecturi);
            var _connection = factory.CreateConnection();
            _connection.Start();
            logger.LogInformation($"connected to {factory.BrokerUri} at consumer two");
            var _session = _connection.CreateSession();

            IDestination dest = _session.GetQueue("test-q1");
            using (IMessageConsumer consumer = _session.CreateConsumer(dest))
            {
                IMessage message;
                while ((message = consumer.Receive()) != null)
                {
                    var objectMessage = message as IObjectMessage;
                    if (objectMessage != null)
                    {
                        logger.LogInformation("Reciving files started at: " + DateTime.Now.ToLongDateString());

                        var messageData = objectMessage.Body;
                        Console.WriteLine($"received: {messageData}");
                    }
                    else
                    {
                        logger.LogError("File object is null");
                    }
                }
            }
        }
    }
}
