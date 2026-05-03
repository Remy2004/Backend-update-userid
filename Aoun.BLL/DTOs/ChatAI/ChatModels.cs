namespace Aoun.BLL.DTOs.ChatAI;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}

public class GeminiSettings
{
    public string ApiKey { get; set; } = string.Empty;
}
