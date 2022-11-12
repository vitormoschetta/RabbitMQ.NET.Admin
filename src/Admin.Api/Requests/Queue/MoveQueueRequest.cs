namespace Admin.Api.Requests.Queue
{
    public class MoveMessagesBetweenQueues
    {
        public string SourceQueueName { get; set; } = string.Empty;
        public string DestinationExchangeName { get; set; } = string.Empty;
        public string DestinationRoutingKey { get; set; } = string.Empty;
    }
}