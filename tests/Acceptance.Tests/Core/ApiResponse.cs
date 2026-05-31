using System.Net;
using RestSharp;

namespace Acceptance.Tests.Core;

public record ApiResponse<T>(
    HttpStatusCode StatusCode,
    T? Body,
    string? RawContent,
    RestResponse RawResponse
)
{
    public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode <= 299;
}
