using Acceptance.Tests.Models;
using RestSharp;

namespace Acceptance.Tests.Clients;

public class ActivationApiClient
{

    private readonly RestClient _client;

    public ActivationApiClient(RestClient client)
    {
        _client = client;
    }

    public async Task<RestResponse> GetHealthAsync()
    {
        var request = new RestRequest("/health", Method.Get);
        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> ActivateSimAsync(ActivationRequest activationRequest)
    {
        var request = new RestRequest("activations", Method.Post);
        request.AddJsonBody(activationRequest);

        return await _client.ExecuteAsync(request);
    }
}