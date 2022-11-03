using RabbitMQ.Client;

var queueName = "pagamentos";

Execute();


void Execute()
{
    var factory = GetConnectionFactory();

    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        SendMessage(channel);
    }
}


void SendMessage(IModel channel)
{
    Console.WriteLine("Entre com uma mensagem para ser enviada para a fila:");
    var message = Console.ReadLine();

    if (string.IsNullOrEmpty(message))
    {
        Console.WriteLine("Mensagem inválida");

        SendMessage(channel);
    }
    else
    {
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: null,
            body: body);

        Console.WriteLine("Mensagem enviada com sucesso");

        SendMessage(channel);
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