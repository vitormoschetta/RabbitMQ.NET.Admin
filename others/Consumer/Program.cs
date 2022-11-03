using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var queueName = "pagamentos";

var factory = GetConnectionFactory();

Execute();


void Execute()
{
    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

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
        Password = "123456"
    };
}