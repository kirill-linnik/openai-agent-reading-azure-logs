namespace Backend.Services;

using Azure.Core;
using Microsoft.Identity.Client;
using System.Threading;
using System.Threading.Tasks;

public class AzureLogApiTokenCredentialService(string tenantId, string clientId, string clientSecret) : TokenCredential
{
    private AccessToken token = new("", DateTimeOffset.Now.AddMilliseconds(-1));

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        token = GetToken();
        return token;
    }

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        token = GetToken();
        return new ValueTask<AccessToken>(token);
    }

    private AccessToken GetToken()
    {
        if (token.ExpiresOn > DateTimeOffset.Now)
        {
            return token;
        }

        string[] scopes = ["https://api.loganalytics.io/.default"];

        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
            .Build();

        AuthenticationResult result = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;
        return new AccessToken(result.AccessToken, result.ExpiresOn);
    }
}
