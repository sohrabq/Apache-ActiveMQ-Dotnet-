
using Apache.NMS;
using Apache.NMS.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logFactory = new LoggerFactory();

            var logger = logFactory.CreateLogger<Program>();
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");
            logger.LogInformation($"about to connect to {connecturi}");
            var factory = new Apache.NMS.ActiveMQ.ConnectionFactory(connecturi);
            var _connection = factory.CreateConnection();
            _connection.Start();
            logger.LogInformation($"connected to {factory.BrokerUri}");
            var _session = _connection.CreateSession();

            var dest = SessionUtil.GetDestination(_session, "test-q1");
            using (IMessageProducer producer = _session.CreateProducer(dest))
            {
                var cnt = 1;
                
                while (cnt <= 10)
                {
                    var message = new
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Book {cnt}"
                    };

                    var objectMessage = producer.CreateObjectMessage(System.Text.Json.JsonSerializer.Serialize(message));
                    Thread.Sleep(1000);
                    producer.Send(objectMessage);
                    cnt++;
                    logger.LogInformation($"sent the message {message.Name}");
                }
            }
        }
    }
}
