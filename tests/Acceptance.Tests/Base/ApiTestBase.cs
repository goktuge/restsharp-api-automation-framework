using Acceptance.Tests.Clients;
using RestSharp;
using Acceptance.Tests.Helpers;
using Acceptance.Tests.Models;
using Acceptance.Tests.TestData;
using System.Net;
using Acceptance.Tests.Core;
using NUnit.Framework.Interfaces;

namespace Acceptance.Tests.Base;

public abstract class ApiTestBase
{

    protected ActivationApiClient ActivationApiClient { get; private set; } = null!;
    private RestClient _restClient = null!;
    private ApiCallContext _apiCallContext = null!;

    [SetUp]
    public void SetUp()
    {
        var baseUrl = ConfigurationHelper.GetBaseUrl();
        var timeoutSeconds = ConfigurationHelper.GetTimeoutSeconds();

        var options = new RestClientOptions(baseUrl)
        {
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };

        _restClient = new RestClient(options);

        _apiCallContext = new ApiCallContext();
        var executor = new ApiRequestExecutor(_apiCallContext);

        ActivationApiClient = new ActivationApiClient(_restClient, executor);
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
    public void BaseTearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            ApiLogger.LogCalls(
                TestContext.CurrentContext.Test.Name,
                _apiCallContext.Calls
            );
        }

        _restClient.Dispose();
    }
}