using Acceptance.Tests.Clients;
using RestSharp;
using Acceptance.Tests.Helpers;
using Acceptance.Tests.Models;
using Acceptance.Tests.TestData;
using System.Net;

namespace Acceptance.Tests.Base;

public abstract class ApiTestBase
{

    protected ActivationApiClient ActivationApiClient { get; private set; } = null!;
    private RestClient _restClient = null!;

    [SetUp]
    public void SetUp()
    {
        var baseUrl = ConfigurationHelper.GetBaseUrl();

        _restClient = new RestClient(baseUrl);
        ActivationApiClient = new ActivationApiClient(_restClient);
    }

    protected async Task<(ActivationRequest Request, ActivationResponse Response)> CreateValidActivationAsync()
    {
        var activationRequest = ActivationTestData.ValidActivationRequest();

        var createResponse = await ActivationApiClient.ActivateSimAsync(activationRequest);

        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));

        var createdActivation = JsonHelper.Deserialize<ActivationResponse>(createResponse.Content!);

        return (activationRequest, createdActivation);
    }

    [TearDown]
    public void TearDown()
    {
        _restClient.Dispose();
    }
}