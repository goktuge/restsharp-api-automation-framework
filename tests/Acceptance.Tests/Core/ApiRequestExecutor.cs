using RestSharp;

namespace Acceptance.Tests.Core;

public class ApiRequestExecutor
{
    private readonly ApiCallContext _context;

    public ApiRequestExecutor(ApiCallContext context)
    {
        _context = context;
    }

    public async Task<RestResponse> ExecuteAsync(
        string operationName,
        RestClient client,
        RestRequest request)
    {
        var response = await client.ExecuteAsync(request);

        _context.Add(new ApiCallRecord(
            OperationName: operationName,
            Method: request.Method,
            Resource: request.Resource,
            Response: response
        ));

        return response;
    }
}
