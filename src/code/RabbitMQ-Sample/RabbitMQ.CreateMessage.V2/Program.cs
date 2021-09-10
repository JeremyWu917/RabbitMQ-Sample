using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.CreateMessage.V2
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

            string message = GetMessage(args);
            IBasicProperties properties = channel.CreateBasicProperties();
            // properties.SetPersistent(true); // 这个方法提示过时，不建议使用
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", "task_queue_sayhi", properties, body);
            Console.WriteLine(" set {0}", message);
            Console.ReadKey();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hi, there!");
        }
    }
}
