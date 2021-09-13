using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.CreateMessage.V3.Controllers
{
    public class CreateMessageController : Controller
    {
        [HttpGet("one/{count}")]
        public async Task<ActionResult> One(int count)
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

            //打开连接
            using var connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            //定义队列
            channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            //定义交换机
            channel.ExchangeDeclare(exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null);

            //将队列绑定到交换机上
            channel.QueueBind(queue: queueName,
                exchange: exchangeName,
                routingKey: string.Empty,
                arguments: null);

            //发送队列
            for (int i = 0; i < count; i++)
            {
                string message = $"Task {i}";
                byte[] body = Encoding.UTF8.GetBytes(message);

                var consumer = new EventingBasicConsumer(channel);

                //发送消息
                channel.BasicPublish(exchange: exchangeName,
                    routingKey: string.Empty,
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"消息：{message} 已发送");
            }

            return Ok();
        }
    }
}
