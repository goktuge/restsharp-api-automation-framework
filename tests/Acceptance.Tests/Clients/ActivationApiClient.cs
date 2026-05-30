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

    public async Task<RestResponse> ActivateSimAsync(
        ActivationRequest activationRequest,
        string? token = "test-token",
        string? correlationId = null)
    {
        var request = new RestRequest("activations", Method.Post);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.AddHeader("Authorization", $"Bearer {token}");
        }

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            request.AddHeader("X-Correlation-Id", correlationId);
        }


        request.AddJsonBody(activationRequest);

        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> GetActivationByIdAsync(
    string activationId,
    string? token = "test-token")
    {
        var request = new RestRequest("/activations/{activationId}", Method.Get);

        request.AddUrlSegment("activationId", activationId);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.AddHeader("Authorization", $"Bearer {token}");
        }

        return await _client.ExecuteAsync(request);
    }

    public async Task<RestResponse> GetActivationsAsync(
    string? status = null,
    string? customerId = null,
    string? token = "test-token")
    {
        var request = new RestRequest("/activations", Method.Get);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.AddHeader("Authorization", $"Bearer {token}");
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            request.AddQueryParameter("status", status);
        }

        if (!string.IsNullOrWhiteSpace(customerId))
        {
            request.AddQueryParameter("customerId", customerId);
        }

        return await _client.ExecuteAsync(request);
    }
}