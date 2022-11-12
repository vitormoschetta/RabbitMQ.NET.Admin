using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var exchangeName = "pgto";
var queueName = "pgto";
var routingKey = "pgto.*";


try
{
    Execute();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}


void Execute()
{

    var factory = GetConnectionFactory();

    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        CreateQueue(channel);

        CreateExchange(channel);

        QueueBind(channel);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Mensagem recebida: {message}");
        };

        channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}


void CreateQueue(IModel channel)
{
    channel.QueueDeclare(
        queue: queueName,
        durable: true,
        exclusive: false,
        autoDelete: true);
}


void CreateExchange(IModel channel)
{
    channel.ExchangeDeclare(
        exchange: exchangeName,
        type: ExchangeType.Topic,
        durable: true,
        autoDelete: true);
}


void QueueBind(IModel channel)
{
    channel.QueueBind(
        queue: queueName,
        exchange: exchangeName,
        routingKey: routingKey);
}


ConnectionFactory GetConnectionFactory()
{
    return new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "adminuser",
        Password = "123456",
        ClientProvidedName = "Consumer.Pgto"
    };
}