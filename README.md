# `RabbitMQ` 简介
## 1. 前言

在企业应用系统领域，会面对不同系统之间的通信、集成与整合，尤其当面临异构系统时，这种分布式的调用与通信变得越发重要。其次，系统中一般会有很多对实时性要求不高的但是执行起来比较较耗时的地方，比如发送短信，邮件提醒，更新文章阅读计数，记录用户操作日志等等，如果实时处理的话，在用户访问量比较大的情况下，对系统压力比较大。

面对这些问题，我们一般会将这些请求，放在消息队列 `MQ` 中处理；异构系统之间使用消息进行通讯。

`MQ` 全称为 `Message Queue`, 消息队列（`MQ`）是一种应用程序对应用程序的通信方法。应用程序通过读写出入队列的消息（针对应用程序的数据）来通信，而无需专用连接来链接它们。消息传递指的是程序之间通过在消息中发送数据进行通信，而不是通过直接调用彼此来通信，直接调用通常是用于诸如远程过程调用的技术。排队指的是应用程序通过 队列来通信。队列的使用除去了接收和发送应用程序同时执行的要求。

`MQ` 是消费-生产者模型的一个典型的代表，一端往消息队列中不断写入消息，而另一端则可以读取或者订阅队列中的消息。

`RabbitMQ` 是一个在 `AMQP` 基础上完整的，可复用的企业消息系统。他遵循 `Mozilla Public License` 开源协议。 

消息传递相较文件传递与远程过程调用（`RPC`）而言，似乎更胜一筹，因为它具有更好的平台无关性，并能够很好地支持并发与异步调用。所以如果系统中出现了如下情况:

- 对操作的实时性要求不高，而需要执行的任务极为耗时；
- 存在异构系统间的整合；

一般的可以考虑引入消息队列。对于第一种情况，常常会选择消息队列来处理执行时间较长的任务。引入的消息队列就成了消息处理的缓冲区。消息队列引入的异步通信机制，使得发送方和接收方都不用等待对方返回成功消息，就可以继续执行下面的代码，从而提高了数据处理的能力。尤其是当访问量和数据流量较大的情况下，就可以结合消息队列与后台任务，通过避开高峰期对大数据进行处理，就可以有效降低数据库处理数据的负荷。



## 2. 搭建环境



### 2.1 安装 `Erlang` 语言运行环境

由于 `RabbitMQ` 使用 `Erlang` 语言编写，所有先安装 Erlang 语言运行环境。下面以 `Windows` 系统为例介绍。

#### 2.1.1 下载安装

下载地址：[`Rrlang`](http://www.erlang.org/downloads )

#### 2.1.2 设置环境变量

手动编辑 `path` 加入路径  `C:\Program Files\erl-24.0\bin`

![image-20210910112056480](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910112056480.png)

#### 2.1.3 检测 Erlang 是否安装成功

打开 `cmd` ,输入 `erl` 后回车，如果看到如下的信息，表明安装成功。

![image-20210910112424266](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910112424266.png)



### 2.2 安装 `RabbitMQ` 服务端

#### 2.2.1 下载安装

下载地址：[`RabbitMQ`](https://www.rabbitmq.com/install-windows.html#installer)

![image-20210910112841541](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910112841541.png)

下载后，双击安装，保持默认选择，下一步，完成即可。

#### 2.2.2 配置 `RabbitMQ` 以 `Windows`服务形式

使 `RabbitMQ` 以 `Windows Service` 的方式在后台运行：以**管理员**身份打开 `cmd` 切换到 `RabbitMQ` 安装目录的 `sbin` 目录下分别执行如下语句：

```powershell
cd /
cd C:\Program Files\RabbitMQ Server\rabbitmq_server-3.9.5\sbin
rabbitmq-service install
rabbitmq-service enable
rabbitmq-service start
```

现在 `RabbitMQ` 的服务端已经启动起来了，你可以查看一下 `Windows Services` 查看。

要查看和控制 `RabbitMQ` 服务端的相关状态，可以用 `rabbitmqctl` 这个脚本。

比如查看状态：

```powershell
rabbitmqctl status
```

假如显示 `node` 没有连接上，需要到 `C:\Windows` 目录下，将 `.erlang.cookie` 文件，拷贝到用户目录下 `C:\Users\{用户名}`，这是`Erlang` 的 `Cookie` 文件，允许与 `Erlang` 进行交互。

#### 2.2.3 常用脚本命令

```powershell
::查询服务状态
rabbitmqctl status

::列举虚拟主机列表
rabbitmqctl list_vhosts

::列举用户列表
rabbitmqctl list_users

::添加用户和密码
rabbitmqctl  add_user  hao  abc123

:: 设置权限   
rabbitmqctl  set_permissions  yy  ".*"  ".*"  ".*"

:: 分配用户组
rabbitmqctl  set_user_tags yy administrator

:: 删除guest用户
rabbitmqctl delete_user guest

::修改用户密码
rabbitmqctl change_password {username}  {newpassowrd}
```



## 3. `RabbitMQ` 网页端管理

> 鉴于采用 `rabbitmqctl` 脚本操作的复杂性，这里推荐使用 `web` 界面查看和管理 `RabbitMQ` 服务。



### 3.1 启用 `web` 管理界面

首先，需要使用 `rabbitmqctl` 脚本开启 `rabbitmq_management` 插件，语句如下：

```powershell
rabbitmq-plugins enable rabbitmq_management
```

**注意**：此处需要在 `sbin` 目录下执行（建议以管理员身份运行命令提示符工具）。

###  3.2 打开 `RabbitMQ` `Web` 界面管理界面

网址：http://localhost:15672/，单击打开后，显示如下：

![image-20210910131201367](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910131201367.png)



## 4. `RabbitMQ` 知识拓展

> `RabbitMQ` 是一个消息代理。

他从消息生产者(`producers`)那里接收消息，然后把消息送给消息消费者（`consumer`）在发送和接受之间，他能够根据设置的规则进行路由，缓存和持久化。

一般提到 `RabbitMQ`和消息，都用到一些专有名词。

- 生产(`Producing`)意思就是发送。发送消息的程序就是一个生产者(`producer`)。我们一般用 `"P"` 来表示：

  　![image-20210910131938779](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910131938779.png)

- 队列(`queue`)就是邮箱的名称。消息通过你的应用程序和 `RabbitMQ` 进行传输，它们只能存储在队列（`queue`）中。 队列（`queue`）容量没有限制，你要存储多少消息都可以——基本上是一个无限的缓冲区。多个生产者（`producers`）能够把消息发送给同一个队列，同样，多个消费者（`consumers`）也能从同一个队列（`queue`）中获取数据。队列可以画成这样（图上是队列的名称）：

 　　![image-20210910131909436](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910131909436.png)

- 消费（`Consuming`）和获取消息是一样的意思。一个消费者（`consumer`）就是一个等待获取消息的程序。我们把它画作 `"C"`：

 　　![image-20210910132007755](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910132007755.png)

通常，消息生产者，消息消费者和消息代理不在同一台机器上。



## 5. 在 `C#` 代码中使用 `RabbitMQ`

> 为了简化编程，此处不再重复造轮子，项目中通过 `Nuget`包管理程序给项目安装 `RabbitMQ.Client` 模块

![image-20210910133213254](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910133213254.png)



### 5.1 创建一个 `RabbitMQ` 管理员权限用户

通过 `Web` 端 `RabbitMQ Management` 来创建一个具有 `Administrator` 权限的账户，此处创建一下 `test` 账户：

![image-20210910135126463](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910135126463.png)

![image-20210910135015362](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210910135015362.png)



### 5.2 创建一个消息生产者 `Producer`

代码如下：

```c#
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
```



### 5.3 创建一个消息消费者 `Consumer`

代码如下：

```c#
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

```



### 5.4 轮询分发

使用工作队列的一个好处就是它能够并行的处理队列。如果堆积了很多任务，我们只需要添加更多的工作者（`workers`）就可以了，扩展很简单。

现在，我们先启动两个接收端，等待接受消息，然后通过 `RabbitMQ` `Web` 管理端模拟生产者发送消息，如下图所示。

两个接收端依次接收到了发出的消息。

默认情况下：`RabbitMQ` 会将每个消息按照顺序依次分发给下一个消费者。所以每个消费者接收到的消息个数大致是平均的。 这种消息分发的方式称之为轮询（`round-robin`）。

![RabbitMQ-Publish-Message](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/RabbitMQ-Publish-Message.gif)



### 5.5 消息响应

当处理一个比较耗时得任务的时候，也许想知道消费者（`consumers`）是否运行到一半就挂掉。在当前的代码中，当 `RabbitMQ` 将消息发送给消费者（`consumers`）之后，马上就会将该消息从队列中移除。此时，如果把处理这个消息的工作者（`worker`）停掉，正在处理的这条消息就会丢失。同时，所有发送到这个工作者的还没有处理的消息都会丢失。

我们不想丢失任何任务消息。如果一个工作者（`worker`）挂掉了，我们希望该消息会重新发送给其他的工作者（`worker`）。

为了防止消息丢失，`RabbitMQ` 提供了消息响应（`acknowledgments`）*机制*。消费者会通过一个`ack`（响应），告诉 `RabbitMQ` 已经收到并处理了某条消息，然后 `RabbitMQ` 才会释放并删除这条消息。

如果消费者（`consumer`）挂掉了，没有发送响应，`RabbitMQ` 就会认为消息没有被完全处理，然后重新发送给其他消费者（`consumer`）。这样，即使工作者（`workers`）偶尔的挂掉，也不会丢失消息。

消息是没有超时这个概念的；当工作者与它断开连的时候，`RabbitMQ` 会重新发送消息。这样在处理一个耗时非常长的消息任务的时候就不会出问题了。

消息响应默认是开启的。在之前的例子中使用了`no_ack=True` 标识把它关闭。是时候移除这个标识了，当工作者（`worker`）完成了任务，就发送一个响应。

```c#
chann.BasicConsume("sayhi", false, consumer);

while (true)
{
    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

    var body = ea.Body;
    var msg = Encoding.UTF8.GetString(body.ToArray());

    int dots = msg.Split('.').Length - 1;
    Thread.Sleep(dots * 1000);

    Console.WriteLine("Received: {0}", msg);
    Console.WriteLine("Done");

    channel.BasicAck(ea.DeliveryTag, false);
}
```

现在,可以保证,即使正在处理消息的工作者被停掉,这些消息也不会丢失,所有没有被应答的消息会被重新发送给其他工作者。

一个很常见的错误就是忘掉了 `BasicAck` 这个方法,这个错误很常见,但是后果很严重. 当客户端退出时,待处理的消息就会被重新分发,但是`RabitMQ` 会消耗越来越多的内存,因为这些没有被应答的消息不能够被释放。



### 5.6 消息持久化

前面已经搞定了即使消费者 `down` 掉，任务也不会丢失，但是，如果 `RabbitMQ Server` 停掉了，那么这些消息还是会丢失。

当 `RabbitMQ Server` 关闭或者崩溃，那么里面存储的队列和消息默认是不会保存下来的。如果要让 `RabbitMQ` 保存住消息，需要在两个地方同时设置：需要保证队列和消息都是持久化的。

首先，要保证 `RabbitMQ` 不会丢失队列，所以要做如下设置：

```c#
bool durable = true;
chann.QueueDeclare("sayhi", durable, false, false, null);
```

 虽然在语法上是正确的，但是在目前阶段是不正确的，因为我们之前已经定义了一个非持久化的 `sayhi` 队列。`RabbitMQ` 不允许我们使用不同的参数重新定义一个已经存在的同名队列，如果这样做就会报错。现在，定义另外一个不同名称的队列：

```c#
bool durable = true;
chann.queueDeclare("task_queue_sayhi", durable, false, false, null);
```

 `queueDeclare`  这个改动需要在发送端和接收端同时设置。

现在保证了 `task_queue` 这个消息队列即使在 `RabbitMQ Server` 重启之后，队列也不会丢失。 然后需要保证消息也是持久化的， 这可以通过设置 `IBasicProperties.DeliveryMode` 为 `2` 来实现：

```c#
var properties = chann.CreateBasicProperties();
// properties.SetPersistent(true);
properties.DeliveryMode = 2;
```

`DeliveryMode` 等于 `2` 就说明这个消息是 `persistent` 的。`1` 是默认是，不是持久的。

需要注意的是，将消息设置为持久化并不能完全保证消息不丢失。虽然他告诉 `RabbitMQ` 将消息保存到磁盘上，但是在 `RabbitMQ` 接收到消息和将其保存到磁盘上这之间仍然有一个小的时间窗口。 `RabbitMQ`  可能只是将消息保存到了缓存中，并没有将其写入到磁盘上。持久化是不能够一定保证的，但是对于一个简单任务队列来说已经足够。如果需要消息队列持久化的强保证，可以使用 `publisher confirms` 。



### 5.7 公平分发

你可能会注意到，消息的分发可能并没有如我们想要的那样公平分配。比如，对于两个工作者。当奇数个消息的任务比较重，但是偶数个消息任务比较轻时，奇数个工作者始终处理忙碌状态，而偶数个工作者始终处理空闲状态。但是 `RabbitMQ` 并不知道这些，他仍然会平均依次的分发消息。

为了改变这一状态，我们可以使用 `basicQos` 方法，设置 `perfetchCount=1` 。这样就告诉 `RabbitMQ` 不要在同一时间给一个工作者发送多于 `1` 个的消息，或者换句话说。在一个工作者还在处理消息，并且没有响应消息之前，不要给他分发新的消息。相反，将这条新的消息发送给下一个不那么忙碌的工作者。

```c#
chann.BasicQos(0, 1, false); 
```



## 6. 完整实例

### 6.1 生产者代码

```c#
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
            var properties = channel.CreateBasicProperties();
            // properties.SetPersistent(true);
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
```

### 6.2 消费者代码

```c#
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

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

            // while (true)
            {
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());

                    Console.WriteLine("Received {0}", message);
                    Console.WriteLine("Done");

                    channel.BasicAck(ea.DeliveryTag, false);
                };                
            }
        }
    }
}
```

