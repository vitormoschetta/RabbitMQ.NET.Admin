namespace Admin.Api;

public class AppSettings
{
    public RabbitMqSettings RabbitMq { get; set; } = new RabbitMqSettings();
}

public class RabbitMqSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}