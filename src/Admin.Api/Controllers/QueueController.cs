using Admin.Api.Requests.Queue;
using Admin.Api.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace Admin.Api.Controllers
{
    [ApiController]
    [Route("api/queue")]
    public class QueueController : ControllerBase
    {
        private readonly ConnectionBrokerService _connectionBroker;

        public QueueController(ConnectionBrokerService connectionBroker)
        {
            _connectionBroker = connectionBroker;
        }

        [HttpPost]
        public IActionResult QueueCreate([FromBody] CreateQueueRequest request)
        {
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

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
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

            channel.QueueDelete(name);

            return Ok();
        }


        /// <summary>
        /// Purge significa eliminar todas as mensagens da fila.
        /// </summary>
        [HttpPut("purge/{name}")]
        public IActionResult QueuePurge(string name)
        {
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

            channel.QueuePurge(name);

            return Ok();
        }


        /// <summary>
        /// Bind significa associar uma fila a um exchange.
        /// </summary>
        [HttpPut("bind")]
        public IActionResult QueueBind([FromBody] BindQueueRequest request)
        {
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

            channel.QueueBind(
                queue: request.QueueName,
                exchange: request.ExchangeName,
                routingKey: request.RoutingKey,
                arguments: request.Arguments);

            return Ok();
        }
    }
}