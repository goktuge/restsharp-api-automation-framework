using Acceptance.Tests.Mocks;

namespace Acceptance.Tests;

[SetUpFixture]
public class TestRunSetup
{
    public static ProvisioningMockServer ProvisioningMock { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        ProvisioningMock = new ProvisioningMockServer();
        ProvisioningMock.StubSuccessfulProvisioning();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ProvisioningMock.Dispose();
    }
}
