using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var exchangeName = "pgto";
var queueName = "pgto";
var routingKey = "pgto.*";


Execute();


void Execute()
{

    var factory = GetConnectionFactory();

    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        channel.QueueDeclare(
           queue: queueName,
           durable: true,
           exclusive: false,
           autoDelete: true);

        channel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: true);

        channel.QueueBind(
            queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey);

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