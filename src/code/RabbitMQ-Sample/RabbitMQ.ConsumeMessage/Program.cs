using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.ConsumeMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            // 创建一个连接实例
            // 服务地址、用户名、密码
            var fac = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "test",
                Password = "123"
            };

            // 开始连接
            using var conn = fac.CreateConnection();
            // 创建一个 Model
            using var chann = conn.CreateModel();
            // 创建一个 sayhi 的消息队列
            chann.QueueDeclare("sayhi", false, false, false, null);
            // 创建一个消费者
            var consumer = new EventingBasicConsumer(chann);
            // 开始消费
            chann.BasicConsume("sayhi", false, consumer);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var msg = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine("Received： {0}", msg);
            };
            Console.ReadLine();
        }
    }
}
