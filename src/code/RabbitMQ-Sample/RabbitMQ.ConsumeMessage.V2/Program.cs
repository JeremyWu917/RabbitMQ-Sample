using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.ConsumeMessage.V2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "test",
                Password = "123"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            bool durable = true;
            channel.QueueDeclare("task_queue_sayhi", durable, false, false, null);
            channel.BasicQos(0, 1, false);
            // Removed deprecated QueueingBasicConsumer and IQueueingBasicConsumer
            // https://github.com/rabbitmq/rabbitmq-dotnet-client/pull/469
            // var consumer = new QueueingBasicConsumer(channel);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("task_queue_sayhi", false, consumer);

            while (true)
            {
                
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();//接收消息并出列

                var body = ea.Body;//消息主体
                var message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine("Received {0}", message);
                channel.BasicAck(ea.DeliveryTag, false);
                if (message == "exit")
                {
                    Console.WriteLine("exit!");
                    break;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
