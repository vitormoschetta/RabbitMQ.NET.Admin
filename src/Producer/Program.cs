using RabbitMQ.Client;

var exchangeName = "pgto";
var queueName = "pgto";

var dlqExchangeName = "pgto.dql";
var dlqQueueName = "pgto.dlq";


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
        CreateDlqQueue(channel);
        CreateDlxExchange(channel);
        BindDlqQueueToDqlExchange(channel);

        CreateQueue(channel);
        CreateExchange(channel);
        BindQueueToExchange(channel, routingKey);

        SendMessage(channel, routingKey);
    }
}


void CreateDlqQueue(IModel channel)
{
    channel.QueueDeclare(
        queue: dlqQueueName,
        durable: true,
        exclusive: false,
        autoDelete: true);
}

void CreateDlxExchange(IModel channel)
{
    channel.ExchangeDeclare(
        exchange: dlqExchangeName,
        type: ExchangeType.Fanout,
        durable: true,
        autoDelete: true);
}

void BindDlqQueueToDqlExchange(IModel channel)
{
    channel.QueueBind(
        queue: dlqQueueName,
        exchange: dlqExchangeName,
        routingKey: string.Empty);
}


void CreateQueue(IModel channel)
{
    channel.QueueDeclare(
        queue: queueName,
        durable: true,
        exclusive: false,
        autoDelete: true,
        arguments: new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", dlqExchangeName }
        });
}

void CreateExchange(IModel channel)
{
    channel.ExchangeDeclare(
        exchange: exchangeName,
        type: ExchangeType.Topic, // Esse é o tipo de exchange que vamos utilizar para distribuir as mensagens para diversas filas, de acordo com a routing key
        durable: true,
        autoDelete: true);
}

void BindQueueToExchange(IModel channel, string routingKey)
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

        // publicar mensagem com prazo de expiração de 10 segundos
        var properties = channel.CreateBasicProperties();
        properties.Expiration = "10000";

        channel.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: properties,
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