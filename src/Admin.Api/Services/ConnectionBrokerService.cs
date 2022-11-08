using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Admin.Api.Services
{
    public class ConnectionBrokerService
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;

        public ConnectionBrokerService(IOptions<AppSettings> appSettings)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = appSettings.Value.RabbitMq.Host,
                UserName = appSettings.Value.RabbitMq.Username,
                Password = appSettings.Value.RabbitMq.Password,
                ClientProvidedName = appSettings.Value.RabbitMq.ClientProvidedName  
            };

            _connection = _connectionFactory.CreateConnection();
        }

        public IConnection GetConnection()
        {
            if (_connection.IsOpen)
            {
                return _connection;
            }

            _connection = _connectionFactory.CreateConnection();
            return _connection;
        }
    }
}