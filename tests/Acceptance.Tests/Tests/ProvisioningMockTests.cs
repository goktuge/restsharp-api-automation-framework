using System.Net;
using Acceptance.Tests.Base;
using Acceptance.Tests;
using Acceptance.Tests.TestData;

namespace Acceptance.Tests;

public class ProvisioningMockTests : ApiTestBase
{
    [TearDown]
    public void ResetProvisioningMock()
    {
        TestRunSetup.ProvisioningMock.StubSuccessfulProvisioning();
    }

    [Test]
    public async Task Should_accept_activation_when_provisioning_provider_succeeds()
    {
        TestRunSetup.ProvisioningMock.StubSuccessfulProvisioning();

        var activationRequest = ActivationTestData.ValidActivationRequest();

        var response = await ActivationApiClient.ActivateSimAsync(activationRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
    }

    [Test]
    public async Task Should_return_bad_gateway_when_provisioning_provider_fails()
    {
        TestRunSetup.ProvisioningMock.StubProvisioningFailure();

        var activationRequest = ActivationTestData.ValidActivationRequest();

        var response = await ActivationApiClient.ActivateSimAsync(activationRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
    }
}
