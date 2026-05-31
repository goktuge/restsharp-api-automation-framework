using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Acceptance.Tests.Mocks;

public class ProvisioningMockServer : IDisposable
{
    private readonly WireMockServer _server;

    public string Url => _server.Url!;

    public ProvisioningMockServer()
    {
        _server = WireMockServer.Start(9099);
    }

    public void StubSuccessfulProvisioning()
    {
        _server.ResetMappings();

        _server
            .Given(
                Request.Create()
                    .WithPath("/provision")
                    .UsingPost()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("""
                    {
                      "providerStatus": "Provisioned"
                    }
                    """)
            );
    }

    public void StubProvisioningFailure()
    {
        _server.ResetMappings();

        _server
            .Given(
                Request.Create()
                    .WithPath("/provision")
                    .UsingPost()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(500)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("""
                    {
                      "error": "Provisioning provider unavailable"
                    }
                    """)
            );
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}
