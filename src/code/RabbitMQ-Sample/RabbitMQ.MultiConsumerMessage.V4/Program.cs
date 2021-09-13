using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.MultiConsumerMessage.V4
{
    class Program
    {
        static void Main(string[] args)
        {
            string queueName = "queue_demo_multi";
            string smsQueueName = "queue_demo_multi_sms";
            string emailQueueName = "queue_demo_multi_eamil";
            string exchangeName = "exchange_demo_multi";
            string hostName = "localhost";
            string userName = "test";
            string password = "123";
            int port = 5672;

            //先创建连接
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };

            using var connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            string[] strs = new string[3];
            strs[0] = queueName;
            strs[1] = smsQueueName;
            strs[2] = emailQueueName;

            Console.Write("输入索引 0 ~ 2 ：");
            int index = Convert.ToInt32(Console.ReadLine());

            channel.ExchangeDeclare(exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null);

            channel.QueueDeclare(queue: strs[index],
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: strs[index],
                exchange: exchangeName,
                routingKey: string.Empty,
                arguments: null);

            //定义消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine($"消费者 {strs[index]} 接收消息 {message}");
            };

            //启动消费者
            channel.BasicConsume(queue: strs[index],
                autoAck: true,//自动确认
                consumer: consumer);


            //处理完消息后，保持程序继续运行，可以继续接收消息
            Console.ReadLine();
        }
    }
}
