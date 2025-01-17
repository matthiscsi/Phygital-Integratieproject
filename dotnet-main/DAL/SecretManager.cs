using Google.Cloud.SecretManager.V1;

namespace PIP.DAL;

public class SecretManager
{
    private readonly SecretManagerServiceClient _client;
    //private readonly string _projectId:
    private readonly string _projectId = "686109148881";

    public SecretManager()
    {
        _client = SecretManagerServiceClient.Create();
    }

    public async Task<string> GetSecretAsync(string secretId)
    {
        SecretVersionName secretVersionName = new SecretVersionName(_projectId, secretId, "latest");

        AccessSecretVersionResponse response = await _client.AccessSecretVersionAsync(secretVersionName);
        string payload = response.Payload.Data.ToStringUtf8();

        return payload;
    }

}