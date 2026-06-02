using Acceptance.Tests.Models;
using RestSharp;
using Acceptance.Tests.Core;
using Acceptance.Tests.Helpers;

namespace Acceptance.Tests.Clients;

public class ActivationApiClient
{

    private readonly RestClient _client;
    private readonly ApiRequestExecutor _executor;

    public ActivationApiClient(
        RestClient client,
        ApiRequestExecutor? executor = null)
    {
        _client = client;
        _executor = executor ?? new ApiRequestExecutor(new ApiCallContext());
    }

    public async Task<RestResponse> GetHealthAsync()
    {
        var request = new RestRequest("/health", Method.Get);
        return await _executor.ExecuteAsync(
            "GET /health",
            _client,
            request
        );
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

        return await _executor.ExecuteAsync(
            "POST /activations",
            _client,
            request
        );
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

        return await _executor.ExecuteAsync(
            "GET /activations",
            _client,
            request
        );
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

        return await _executor.ExecuteAsync(
            "GET /activations/{activationId}",
            _client,
            request
        );
    }

    public async Task<RestResponse> UpdateActivationStatusAsync(
        string activationId,
        UpdateActivationStatusRequest updateRequest,
        string? token = "test-token"
    )
    {
        var request = new RestRequest("/activations/{activationId}/status", Method.Patch);

        request.AddUrlSegment("activationId", activationId);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.AddHeader("Authorization", $"Bearer {token}");
        }

        request.AddJsonBody(updateRequest);

        return await _executor.ExecuteAsync(
            "PATCH /activations/{activationId}/status",
            _client,
            request
        );
    }

    public async Task<RestResponse> DeleteActivationAsync(
    string activationId,
    string? token = "test-token")
    {
        var request = new RestRequest("/activations/{activationId}", Method.Delete);

        request.AddUrlSegment("activationId", activationId);


        if (!string.IsNullOrWhiteSpace(token))
        {
            request.AddHeader("Authorization", $"Bearer {token}");
        }

        return await _executor.ExecuteAsync(
            "DELETE /activations/{activationId}",
            _client,
            request
        );
    }

    public async Task<ApiResponse<TResponse>> ActivateSimTypedAsync<TResponse>(
    ActivationRequest activationRequest,
    string? token = "test-token",
    string? correlationId = null)
    {
        var response = await ActivateSimAsync(
            activationRequest,
            token,
            correlationId
        );

        return ApiResponseMapper.ToApiResponse<TResponse>(response);
    }

    public async Task<ApiResponse<TResponse>> GetActivationByIdTypedAsync<TResponse>(
    string activationId,
    string? token = "test-token")
    {
        var request = new RestRequest("/activations/{activationId}", Method.Get);
        request.AddUrlSegment("activationId", activationId);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.AddHeader("Authorization", $"Bearer {token}");
        }

        return await _executor.ExecuteTypedAsync<TResponse>(
            "GET /activations/{activationId}",
            _client,
            request
        );
    }

    public async Task<ApiResponse<TResponse>> GetActivationsTypedAsync<TResponse>(
    string? status = null,
    string? customerId = null,
    string? token = "test-token")
    {
        var response = await GetActivationsAsync(status, customerId, token);

        return ApiResponseMapper.ToApiResponse<TResponse>(response);
    }

    public async Task<ApiResponse<TResponse>> UpdateActivationStatusTypedAsync<TResponse>(
    string activationId,
    UpdateActivationStatusRequest updateRequest,
    string? token = "test-token")
    {
        var response = await UpdateActivationStatusAsync(
            activationId,
            updateRequest,
            token
        );

        return ApiResponseMapper.ToApiResponse<TResponse>(response);
    }
}