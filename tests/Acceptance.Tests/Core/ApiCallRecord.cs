using RestSharp;

namespace Acceptance.Tests.Core;

public record ApiCallRecord(
    string OperationName,
    Method Method,
    string Resource,
    RestResponse Response
);
