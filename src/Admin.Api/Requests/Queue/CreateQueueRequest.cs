namespace Admin.Api.Requests.Queue
{
    public class CreateQueueRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}