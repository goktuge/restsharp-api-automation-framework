namespace Acceptance.Tests.Core;

public class ApiCallContext
{
    private readonly List<ApiCallRecord> _calls = new();

    public IReadOnlyList<ApiCallRecord> Calls => _calls;

    public void Add(ApiCallRecord call)
    {
        _calls.Add(call);
    }
}
