namespace Admin.Api.Requests.Exchange
{
    public class CreateExchangeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Durable { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}