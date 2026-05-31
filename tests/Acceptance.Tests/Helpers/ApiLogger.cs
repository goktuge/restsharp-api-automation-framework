using Acceptance.Tests.Core;
using NUnit.Framework;

namespace Acceptance.Tests.Helpers;

public static class ApiLogger
{
    public static void LogCalls(string testName, IReadOnlyList<ApiCallRecord> calls)
    {
        TestContext.Out.WriteLine("");
        TestContext.Out.WriteLine("========== API FAILURE LOG ==========");
        TestContext.Out.WriteLine($"Test: {testName}");

        if (calls.Count == 0)
        {
            TestContext.Out.WriteLine("No API calls were captured.");
            TestContext.Out.WriteLine("=====================================");
            return;
        }

        foreach (var call in calls)
        {
            LogCall(call);
        }

        TestContext.Out.WriteLine("=====================================");
        TestContext.Out.WriteLine("");
    }

    private static void LogCall(ApiCallRecord call)
    {
        var response = call.Response;

        TestContext.Out.WriteLine("----- API CALL -----");
        TestContext.Out.WriteLine($"Operation: {call.OperationName}");
        TestContext.Out.WriteLine($"Request: {call.Method} {call.Resource}");
        TestContext.Out.WriteLine($"Response URI: {response.ResponseUri}");
        TestContext.Out.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
        TestContext.Out.WriteLine($"Response Status: {response.ResponseStatus}");
        TestContext.Out.WriteLine($"Content: {response.Content ?? "<empty>"}");
    }
}
