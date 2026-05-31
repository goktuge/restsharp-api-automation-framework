using Acceptance.Tests.Core;
using RestSharp;

namespace Acceptance.Tests.Helpers;

public static class ApiResponseMapper
{
    public static ApiResponse<T> ToApiResponse<T>(RestResponse response)
    {
        T? body = default;

        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            body = JsonHelper.Deserialize<T>(response.Content);
        }

        return new ApiResponse<T>(
            StatusCode: response.StatusCode,
            Body: body,
            RawContent: response.Content,
            RawResponse: response
        );
    }
}
