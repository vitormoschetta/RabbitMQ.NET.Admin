using RabbitMQ.Client;

var exchangeName = "pgto";
var queueName = "pgto";


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
    Console.WriteLine("Escolha uma RouteKey: ");
    Console.WriteLine("1 - pgto.cartao");
    Console.WriteLine("2 - pgto.boleto");
    var opcao = Console.ReadLine();

    if (opcao == "1")
    {
        var routeKey = "pgto.cartao";
        BuildConnection(routeKey);
    }
    else if (opcao == "2")
    {
        var routeKey = "pgto.boleto";
        BuildConnection(routeKey);
    }
    else
    {
        Console.WriteLine("Opção inválida");
        Execute();
    }
}


void BuildConnection(string routingKey)
{
    var factory = GetConnectionFactory();

    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        CreateQueue(channel);

        CreateExchange(channel);

        QueueBind(channel, routingKey);

        SendMessage(channel, routingKey);
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


void QueueBind(IModel channel, string routingKey)
{
    channel.QueueBind(
        queue: queueName,
        exchange: exchangeName,
        routingKey: routingKey);
}



void SendMessage(IModel channel, string routingKey)
{
    Console.WriteLine("Entre com uma mensagem para ser enviada para a fila:");
    var message = Console.ReadLine();

    if (string.IsNullOrEmpty(message))
    {
        Console.WriteLine("Mensagem inválida");

        SendMessage(channel, routingKey);
    }
    else
    {
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: body);

        Console.WriteLine("Mensagem enviada com sucesso");

        SendMessage(channel, routingKey);
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
        ClientProvidedName = "Producer01"
    };
}