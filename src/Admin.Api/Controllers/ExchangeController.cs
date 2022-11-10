using Admin.Api.Requests.Exchange;
using Admin.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Api.Controllers
{
    [ApiController]
    [Route("api/queue")]
    public class ExchangeController : ControllerBase
    {
        private readonly ConnectionBrokerService _connectionBroker;

        public ExchangeController(ConnectionBrokerService connectionBroker)
        {
            _connectionBroker = connectionBroker;
        }

        [HttpPost]
        public IActionResult ExchangeCreate([FromBody] CreateExchangeRequest request)
        {
            var connection = _connectionBroker.GetConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: request.Name,
                type: request.Type,
                durable: request.Durable,
                autoDelete: request.AutoDelete,
                arguments: request.Arguments);

            return Ok();
        }        
    }
}