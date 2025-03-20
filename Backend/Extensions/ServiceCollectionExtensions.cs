using Azure;
using Azure.Communication;
using Azure.Communication.Chat;
using Azure.Communication.Identity;
using Backend.Services;
using Microsoft.SemanticKernel;

namespace Backend.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var azureOpenAiServiceEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            ArgumentException.ThrowIfNullOrEmpty(azureOpenAiServiceEndpoint);
            var azureOpenAiServiceApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            ArgumentException.ThrowIfNullOrEmpty(azureOpenAiServiceApiKey);
            var deployedModelName = Environment.GetEnvironmentVariable("AZURE_OPENAI_CHATGPT_DEPLOYMENT");
            ArgumentException.ThrowIfNullOrWhiteSpace(deployedModelName);

            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder = kernelBuilder.AddAzureOpenAIChatCompletion(deployedModelName, azureOpenAiServiceEndpoint, azureOpenAiServiceApiKey);

            return kernelBuilder.Build();
        });

        services.AddSingleton(sp =>
        {
            var tenantId = Environment.GetEnvironmentVariable("AZURE_APP_TENANT_ID");
            ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
            var clientId = Environment.GetEnvironmentVariable("AZURE_APP_CLIENT_ID");
            ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
            var clientSecret = Environment.GetEnvironmentVariable("AZURE_APP_CLIENT_SECRET");
            ArgumentException.ThrowIfNullOrWhiteSpace(clientSecret);

            return new AzureLogApiTokenCredentialService(tenantId, clientId, clientSecret);
        });

        services.AddSingleton(sp =>
        {
            var azureMetricTokenCredentialService = sp.GetRequiredService<AzureLogApiTokenCredentialService>();
            var logger = sp.GetRequiredService<ILogger<LogAnalyzerQueryService>>();
            var workspaceId = Environment.GetEnvironmentVariable("AZURE_LOG_ANALYTICS_WORKSPACE_ID");
            ArgumentException.ThrowIfNullOrWhiteSpace(workspaceId);

            return new LogAnalyzerQueryService(azureMetricTokenCredentialService, logger, workspaceId);
        });

        services.AddSingleton(sp =>
        {
            var acsChatEndpoint = Environment.GetEnvironmentVariable("AZURE_CHAT_ENDPOINT");
            ArgumentException.ThrowIfNullOrWhiteSpace(acsChatEndpoint);
            var acsChatAccessKey = Environment.GetEnvironmentVariable("AZURE_CHAT_ACCESS_KEY");
            ArgumentException.ThrowIfNullOrWhiteSpace(acsChatAccessKey);

            var acsResourceUri = new Uri(acsChatEndpoint);
            var identityClient = new CommunicationIdentityClient(acsResourceUri, new AzureKeyCredential(acsChatAccessKey));

            var identityResponse = identityClient.CreateUser();
            var identity = identityResponse.Value;

            var tokenOptions = new CommunicationTokenRefreshOptions(true, (cancellationToken) =>
            {
                var newTokenResponse = identityClient.GetToken(identity, scopes: [CommunicationTokenScope.Chat], cancellationToken);
                return newTokenResponse.Value.Token;
            });

            var chatClient = new ChatClient(acsResourceUri, new CommunicationTokenCredential(tokenOptions));

            return new ChatService(identityClient, chatClient);
        });

        services.AddSingleton(sp =>
        {
            var kernel = sp.GetRequiredService<Kernel>();
            var logAnalyzerQueryService = sp.GetRequiredService<LogAnalyzerQueryService>();
            var chatService = sp.GetRequiredService<ChatService>();
            var logger = sp.GetRequiredService<ILogger<ChatCompletionService>>();
            var resourceId = Environment.GetEnvironmentVariable("AZURE_RESOURCE_ID");
            ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);

            return new ChatCompletionService(kernel, logAnalyzerQueryService, chatService, logger, resourceId);
        });

        return services;
    }
}
