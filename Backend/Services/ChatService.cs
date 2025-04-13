using Azure;
using Azure.Communication.Chat;
using Azure.Communication.Identity;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Backend.Services;

public class ChatService(CommunicationIdentityClient identityClient, ChatClient chatClient)
{

    private ISet<string> _chatThreads = new HashSet<string>();

    public async Task<string> CreateChatAsync()
    {
        var userResponse = await identityClient.CreateUserAsync();
        var assistantResponse = await identityClient.CreateUserAsync();
        var userTokenResponse = await identityClient.GetTokenAsync(userResponse.Value, new[] { CommunicationTokenScope.Chat });
        var assitantResponse = await identityClient.GetTokenAsync(assistantResponse.Value, new[] { CommunicationTokenScope.Chat });

        var user = new ChatParticipant(userResponse.Value);
        user.DisplayName = AuthorRole.User.Label;

        var assistant = new ChatParticipant(assistantResponse.Value);
        assistant.DisplayName = AuthorRole.Assistant.Label;

        var chatThreadClient = await chatClient.CreateChatThreadAsync("ACS Email Resource Discovery",
            participants: new[] { user, assistant }); // Fixed array initialization syntax

        var chatThreadId = chatThreadClient.Value.ChatThread.Id;
        _chatThreads.Add(chatThreadId);

        return chatThreadId;
    }

    public async Task<string> GetThreadTopic(string chatThreadId)
    {
        var chatThreadClient = GetChatThreadClient(chatThreadId);
        var properties = await chatThreadClient.GetPropertiesAsync();
        return properties.Value.Topic;
    }

    public async Task UpdateThreadTopic(string chatThreadId, string threadTopic)
    {
        var chatThreadClient = GetChatThreadClient(chatThreadId);
        await chatThreadClient.UpdateTopicAsync(threadTopic);
    }

    public async Task SendUserMessageAsync(string chatThreadId, string userMessage)
    {
        await SendMessageAsync(chatThreadId, userMessage, AuthorRole.User);
    }

    public async Task SendAssistantMessageAsync(string chatThreadId, string assistantMessage)
    {
        await SendMessageAsync(chatThreadId, assistantMessage, AuthorRole.Assistant);
    }

    public async Task<ChatHistory> GetFreshMessagesAsync(string chatThreadId, string? lastMessageId = null)
    {
        var chatThreadClient = GetChatThreadClient(chatThreadId);
        AsyncPageable<Azure.Communication.Chat.ChatMessage> messages;
        if (!string.IsNullOrEmpty(lastMessageId))
        {
            var message = await chatThreadClient.GetMessageAsync(lastMessageId);
            var createdOn = message.Value.CreatedOn;
            messages = chatThreadClient.GetMessagesAsync(createdOn);
        }
        else
        {
            messages = chatThreadClient.GetMessagesAsync();
        }

        return await ExtractMessagesAsync(messages);
    }

    public async Task<ChatHistory> GetAllMessages(string chatThreadId)
    {
        return await GetFreshMessagesAsync(chatThreadId);
    }

    private ChatThreadClient GetChatThreadClient(string chatThreadId)
    {
        if (!_chatThreads.Contains(chatThreadId))
        {
            throw new ArgumentException("Chat thread not found");
        }
        return chatClient.GetChatThreadClient(chatThreadId);
    }

    private async Task SendMessageAsync(string chatThreadId, string message, AuthorRole role)
    {
        var chatThreadClient = GetChatThreadClient(chatThreadId);
        await chatThreadClient.SendMessageAsync(content: message, senderDisplayName: role.Label);
    }

    private async Task<ChatHistory> ExtractMessagesAsync(AsyncPageable<Azure.Communication.Chat.ChatMessage> messages)
    {
        var chatHistory = new ChatHistory();
        await foreach (var message in messages)
        {
            var role = message.SenderDisplayName;
            if (string.IsNullOrEmpty(role)) // Skip technical messages
            {
                continue;
            }

            chatHistory.AddMessage(
                authorRole: role.Equals(AuthorRole.User.Label) ? AuthorRole.User : AuthorRole.Assistant,
                content: message.Content.Message,
                metadata: new Dictionary<string, object?>
                {
                    { "id", message.Id },
                    { "createdOn", message.CreatedOn }
                }
            );
        }

        return chatHistory;
    }

}
