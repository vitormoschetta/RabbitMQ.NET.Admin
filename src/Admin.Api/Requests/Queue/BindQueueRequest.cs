namespace Admin.Api.Requests.Queue
{
    public class BindQueueRequest
    {
        public string QueueName { get; set; } = string.Empty;
        public string ExchangeName { get; set; } = string.Empty;
        public string RoutingKey { get; set; } = string.Empty;
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}