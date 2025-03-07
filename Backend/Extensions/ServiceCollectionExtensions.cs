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

        services.AddSingleton<ConversationContextService>();

        services.AddSingleton(sp =>
        {
            var kernel = sp.GetRequiredService<Kernel>();
            var logAnalyzerQueryService = sp.GetRequiredService<LogAnalyzerQueryService>();
            var conversationContextService = sp.GetRequiredService<ConversationContextService>();
            var logger = sp.GetRequiredService<ILogger<ChatCompletionService>>();
            var resourceId = Environment.GetEnvironmentVariable("AZURE_RESOURCE_ID");
            ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);

            return new ChatCompletionService(kernel, logAnalyzerQueryService, conversationContextService, logger, resourceId);
        });

        return services;
    }
}
