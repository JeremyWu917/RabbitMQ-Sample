using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.CreateMessage
{
    // 创建一个消息生产者
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
            // 消息内容
            string msg = "Hi, there! This is jeremy wu!";
            // 编码格式
            var body = Encoding.UTF8.GetBytes(msg);
            // 开始传递的队列
            chann.BasicPublish("", "sayhi", null, body);
            Console.WriteLine("Sended: {0}", msg);
            Console.ReadKey();
        }
    }
}
