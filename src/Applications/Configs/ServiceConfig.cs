namespace Applications.Configs;

public class ServiceConfig
{
    public PropostaServiceConfig PropostaService { get; set; } = new();
}

public class PropostaServiceConfig
{
    public string BaseUrl { get; set; } = string.Empty;
}
