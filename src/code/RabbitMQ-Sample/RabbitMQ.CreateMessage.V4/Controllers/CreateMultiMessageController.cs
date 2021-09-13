using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.CreateMessage.V4.Controllers
{
    public class CreateMultiMessageController : Controller
    {
        [HttpGet("multi/{count}")]
        public async Task<ActionResult> Multi(int count)
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

            channel.ExchangeDeclare(exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null);

            //这里声明三个队列，并且绑定同一个交换机
            channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: queueName,
                exchange: exchangeName,
                routingKey: string.Empty,
                arguments: null);

            channel.QueueDeclare(queue: smsQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: smsQueueName,
                exchange: exchangeName,
                routingKey: string.Empty,
                arguments: null);

            channel.QueueDeclare(queue: emailQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: emailQueueName,
                exchange: exchangeName,
                routingKey: string.Empty,
                arguments: null);

            for (int i = 0; i < count; i++)
            {
                string message = $"Task {i}";
                byte[] body = Encoding.UTF8.GetBytes(message);

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
