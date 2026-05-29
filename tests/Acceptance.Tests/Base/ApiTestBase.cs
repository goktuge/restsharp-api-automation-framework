using Acceptance.Tests.Clients;
using RestSharp;
using Acceptance.Tests.Helpers;

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

    [TearDown]
    public void TearDown()
    {
        _restClient.Dispose();
    }
}