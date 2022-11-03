using Admin.Api.Requests.Queue;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Admin.Api.Controllers
{
    [ApiController]
    [Route("api/queue")]
    public class QueueController : ControllerBase
    {
        private readonly ConnectionFactory _connectionFactory;

        public QueueController(IOptions<AppSettings> appSettings)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = appSettings.Value.RabbitMq.Host,
                UserName = appSettings.Value.RabbitMq.Username,
                Password = appSettings.Value.RabbitMq.Password
            };
        }
       
        [HttpPost]
        public IActionResult QueueCreate([FromBody] CreateQueueRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: request.Name,
                durable: request.Durable,
                exclusive: request.Exclusive,
                autoDelete: request.AutoDelete,
                arguments: request.Arguments);

            return Ok();
        }

        [HttpDelete("{name}")]
        public IActionResult QueueDelete(string name)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDelete(name);

            return Ok();
        }

        [HttpPut("purge/{name}")]
        public IActionResult QueuePurge(string name)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueuePurge(name);

            return Ok();
        }

        [HttpPut("bind")]
        public IActionResult QueueBind([FromBody] BindQueueRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueBind(
                queue: request.QueueName,
                exchange: request.ExchangeName,
                routingKey: request.RoutingKey,
                arguments: request.Arguments);

            return Ok();
        }
    }
}