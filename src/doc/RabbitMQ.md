# `RabbitMQ` 高级应用简介

## 1 同步与异步对比

使用 `RabbitMQ` 之前的服务器架构：

![image-20210913085421055](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210913085421055.png)

使用 `RabbitMQ` 之后的服务器架构：

![image-20210913085623111](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210913085623111.png)



## 2 异步化架构流程

> 异步，不是同步，也就是不立即处理，而是延迟处理，以时间换性能

**举例**：
一个请求，需要一秒才能处理完，请求多了，服务器就处理不过来，如果能优化到一毫秒处理一个请求，那就提升了 1000 倍处理能力，但是这是不可能的，因为数据库有瓶颈，那么我们该如何解决此类问题呢？

**解决方案**：

1. 在**应用服务器（ `Web API` ）**和**数据库**之间加一层**消息队列**
2. 从**应用服务器**到**消息队列**只是保存一下操作的信息（在内存），直接返回结果给用户，但是并未完成业务。这是可以达到毫秒级响应，因为没有处理业务逻辑
3. 在**消息队列**和**数据库**之间再加一层**处理器**，处理器就是从消息队列里面拿数据的，然后处理器再去与数据库交互，处理业务逻辑消息应该使用 `JSON`

应用服务器是不能主动去查询数据库再返回结果给用户的，因为成本比较高，一般都是客户端**轮询**或 **`WebSocket`** 之类的，去问应用服务器业务逻辑是否完成，比如扫码之类的功能



## 3 异步化优劣势解读

### 3.1 优势

#### 3.1.1  流量削峰

流量削峰，消息队列的最重要特性，把流量高峰的业务延迟到后面再处理

流量高峰期，请求特别多，但是应用服务器不处理业务逻辑，所以无所谓，吞吐量高，业务堆积在队列里面，晚点再处理

#### 3.1.2 高可用

可用性，就是对外不间断的提供服务。简化应用服务，处理器异常也不影响我们对外提供服务

使用消息队列，应用服务器可能也就做一下数据验证什么的工作，具体的业务逻辑在处理器中处理，即使所有的处理器都宕机，但是我们的消息队列依然可以接收消息，这样，我们的系统依然能对外提供服务

#### 3.1.3 扩展性

消息队列的消息来自应用服务器，如果我们需要做扩展功能和手机业务逻辑，只需要给处理器这一层升级就可以了。用户提交的信息不需要改变，也完全不影响应用服务器，耦合度也降低了

直接从物理层面隔离，扩展互不影响

#### 3.1.4 重试机制

以前我们的应用服务器是直接连接数据库，对数据进行操作，操作失败就返回失败。
消息队列如果失败了，并不会把消息扔掉，之后再重新尝试操作

### 3.2 劣势

#### 3.2.1 降低用户体验

因为不能快速拿到结果，会降低用户体验，需要业务妥协一下

#### 3.2.2 代码的复杂度

原来我们只有客户端、应用服务器、数据库这三层。现在我们有客户端、应用服务器、消息队列、处理器、数据库

应用服务器层，除了做验证和消息序列化操作，还要支持用户查询业务处理结果，处理器层，需要写业务逻辑

#### 3.2.3 重放攻击

页面连点几次，数据库有多条相同数据，但是在消息队列里面就比较难处理了，会有多条重复消息

可能因为网络抖动，操作信息已经保存到消息队列中，但是应用服务器没有返回结果。用户就以为请求没有成功，很可能会再次发起请求，消息队列会出现多条相同的数据

#### 3.2.4 幂等性设计

处理器处理完业务逻辑会返回结果，并删除消息队列中的信息，但是在消息队列里，也有可能处理器对数据库处理完成，但是没有移除消息队列的信息

幂等性设计就是说，处理器处理重复的消息，不会在数据库产生新的数据，也不会影响结果



## 4 消息队列

> 消息队列，是一个独立进程，（一般）使用内存保存（速度快，有丢失），支持网络读写

生产者消费者模式：

- 生产者负责往消息队列里写入数据，生产者可以有多个
- 消费者负责使用消息队列中的数据
- 一条消息消费一次

生产者消费者模式长这样

![image-20210913090841389](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210913090841389.png)

发布订阅模式：

- 发布者负责往消息队列里写入数据，发布者可以有多个
- 订阅者负责使用消息队列中的数据，订阅者可以有多个
- 一条消息订阅多次

发布订阅模式长这样

![image-20210913090915756](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210913090915756.png)



## 5 `RabbitMQ` 

> `RabbitMQ` 相对于一般的消息队列来说，有一个比较独特的设计，就是 **`Exchangeds`（交换机）**和 **`Queues`（队列）**，所以系统结构就变成了，生产者先连接交换机，交换机再去连接消息队列

![image-20210913091139562](https://gitee.com/jeremywuiot/img-res-all/raw/master/src/iie_shop/image-20210913091139562.png)

多了交换机这一层，就可以有很多功能：

- 路由：由交换机去转发到消息队列
- 实现了一条消息多个队列使用：因为消息由交换机转发，所以可以转发到多个队列中



## 6 `RabbitMQ` 核心函数

### 6.1 定义队列

```c#
/// <summary>
/// Declares a queue. See the <a href="https://www.rabbitmq.com/queues.html">Queues guide</a> to learn more.
/// </summary>
/// <param name="queue">The name of the queue. Pass an empty string to make the server generate a name.</param>
/// <param name="durable">Should this queue will survive a broker restart?</param>
/// <param name="exclusive">Should this queue use be limited to its declaring connection? Such a queue will be deleted when its declaring connection closes.</param>
/// <param name="autoDelete">Should this queue be auto-deleted when its last consumer (if any) unsubscribes?</param>
/// <param name="arguments">Optional; additional queue arguments, e.g. "x-queue-type"</param>
QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments);
```

### 6.2 定义交换机

```c#
/// <summary>Declare an exchange.</summary>
/// <remarks>
/// The exchange is declared non-passive and non-internal.
/// The "nowait" option is not exercised.
/// </remarks>
void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments);
```

### 6.3 将队列绑定到交换机上

```C#
/// <summary>
/// Bind a queue to an exchange.
/// </summary>
/// <remarks>
///   <para>
///     Routing key must be shorter than 255 bytes.
///   </para>
/// </remarks>
void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments);
```

### 6.4 发布消息

```c#
/// <summary>
/// (Extension method) Convenience overload of BasicPublish.
/// </summary>
/// <remarks>
/// The publication occurs with mandatory=false
/// </remarks>
public static void BasicPublish(this IModel model, string exchange, string routingKey, IBasicProperties basicProperties, ReadOnlyMemory<byte> body)
{
    model.BasicPublish(exchange, routingKey, false, basicProperties, body);
}

/// <summary>
/// Publishes a message.
/// </summary>
/// <remarks>
///   <para>
///     Routing key must be shorter than 255 bytes.
///   </para>
/// </remarks>
void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, ReadOnlyMemory<byte> body);
```

### 6.5 启动消费者，接收消息

```C#
/// <summary>Start a Basic content-class consumer.</summary>
public static string BasicConsume(this IModel model, string queue, bool autoAck, IBasicConsumer consumer)
{
    return model.BasicConsume(queue, autoAck, "", false, false, null, consumer);
}

/// <summary>Start a Basic content-class consumer.</summary>
string BasicConsume(
    string queue,
    bool autoAck,
    string consumerTag,
    bool noLocal,
    bool exclusive,
    IDictionary<string, object> arguments,
    IBasicConsumer consumer);
```

### 6.6 消费者处理消息（ `Received` 事件）

```C#
///<summary>
/// Event fired when a delivery arrives for the consumer.
/// </summary>
/// <remarks>
/// Handlers must copy or fully use delivery body before returning.
/// Accessing the body at a later point is unsafe as its memory can
/// be already released.
/// </remarks>
public event EventHandler<BasicDeliverEventArgs> Received;
```















































