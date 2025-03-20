using Azure;
using Azure.Communication.Chat;
using Azure.Communication.Identity;
using Backend.Models;

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
        user.DisplayName = ChatRole.User;

        var assistant = new ChatParticipant(assistantResponse.Value);
        assistant.DisplayName = ChatRole.Assistant;

        var chatThreadClient = await chatClient.CreateChatThreadAsync("ACS Email Resource Discovery",
            participants: [user, assistant]);

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
        await SendMessageAsync(chatThreadId, userMessage, ChatRole.User);
    }

    public async Task SendAssistantMessageAsync(string chatThreadId, string assistantMessage)
    {
        await SendMessageAsync(chatThreadId, assistantMessage, ChatRole.Assistant);
    }

    public async Task<IList<Models.ChatMessage>> GetFreshMessagesAsync(string chatThreadId, string? lastMessageId)
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


        var messageList = new List<Models.ChatMessage>();
        await foreach (var newMessage in messages)
        {
            var role = newMessage.SenderDisplayName;
            if (string.IsNullOrEmpty(role)) //we skip all the technical messages here
            {
                continue;
            }

            messageList.Add(new Models.ChatMessage(
                Id: newMessage.Id,
                Role: role,
                Content: newMessage.Content.Message,
                CreatedOn: newMessage.CreatedOn
            ));
        }

        return messageList;
    }

    private ChatThreadClient GetChatThreadClient(string chatThreadId)
    {
        if (!_chatThreads.Contains(chatThreadId))
        {
            throw new ArgumentException("Chat thread not found");
        }
        return chatClient.GetChatThreadClient(chatThreadId);
    }

    private async Task SendMessageAsync(string chatThreadId, string message, string sender)
    {
        var chatThreadClient = GetChatThreadClient(chatThreadId);
        await chatThreadClient.SendMessageAsync(content: message, senderDisplayName: sender);
    }

}
