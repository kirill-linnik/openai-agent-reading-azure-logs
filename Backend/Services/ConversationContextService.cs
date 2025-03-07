namespace Backend.Services;

public class ConversationContextService
{
    private static string _conversationContext = string.Empty;

    public string GetConversationContext()
    {
        return _conversationContext;
    }

    public void SetConversationContext(string conversationContext)
    {
        _conversationContext = conversationContext;
    }
}
