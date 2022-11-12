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


        /// <summary>
        /// Unbind significa desassociar uma fila de um exchange.
        /// </summary>
        [HttpPut("unbind")]
        public IActionResult QueueUnbind([FromBody] BindQueueRequest request)
        {
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

            channel.QueueUnbind(
                queue: request.QueueName,
                exchange: request.ExchangeName,
                routingKey: request.RoutingKey,
                arguments: request.Arguments);

            return Ok();
        }


        /// <summary>
        /// QueueDeclarePassive significa obter as informações sobre mensagens da fila.
        /// </summary>
        [HttpGet("{name}")]
        public IActionResult QueueGet(string name)
        {
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

            var queue = channel.QueueDeclarePassive(name);

            return Ok(queue);
        }


        /// <summary>
        /// Move uma mensagem de uma fila para outra.
        /// </summary>
        [HttpPut("move-message")]
        public IActionResult QueueMove([FromBody] MoveMessagesBetweenQueues request)
        {
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

            var message = channel.BasicGet(request.SourceQueueName, true);

            var properties = channel.CreateBasicProperties();
            properties.Expiration = "10000";

            channel.BasicPublish(
                exchange: request.DestinationExchangeName,
                routingKey: request.DestinationRoutingKey,
                basicProperties: properties,
                body: message.Body);

            return Ok();
        }


        /// <summary>
        /// Move todas as mensagens de uma fila para outra.
        /// </summary>
        [HttpPut("move-all-messages")]
        public IActionResult QueueMoveAll([FromBody] MoveMessagesBetweenQueues request)
        {
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

            while (true)
            {
                var message = channel.BasicGet(request.SourceQueueName, true);

                if (message == null)
                    break;

                var properties = channel.CreateBasicProperties();
                properties.Expiration = "10000";

                channel.BasicPublish(
                    exchange: request.DestinationExchangeName,
                    routingKey: request.DestinationRoutingKey,
                    basicProperties: properties,
                    body: message.Body);
            }

            return Ok();
        }
    }
}