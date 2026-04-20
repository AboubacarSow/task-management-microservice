namespace project_service.Data.Utilities;

public class DatabaseSettings
{
    public string ConnectionStrings { get; set; }=default!;
    public string Database { get; set; } = default!;
    public string ProjectCollection { get; set; }= default!;
}
