using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.ConsumerMessage.V3
{
    class Program
    {
        static void Main(string[] args)
        {
            // 定义队列名称
            string queueName = "queue_demo_one";
            // 定义交换机名称
            string exchangeName = "exchange_demo_one";
            // 定义主机名称
            string hostName = "localhost";
            // 定义用户名
            string userName = "test";
            // 定义密码
            string password = "123";
            // 定义端口号
            int port = 5672;

            // 创建连接
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                //端口，15672 是 web 端管理用的，5672 是用于客户端与消息中间件之间可以传递消息
                Port = port
            };

            using var connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            //定义消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine($"消费者接收消息 {message}");
            };

            //启动消费者
            channel.BasicConsume(queue: queueName,
                autoAck: true,//自动确认
                consumer: consumer);

            //处理完消息后，保持程序继续运行，可以继续接收消息
            Console.ReadLine();
        }
    }
}
