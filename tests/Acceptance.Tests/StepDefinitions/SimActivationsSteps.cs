using System.Net;
using Acceptance.Tests.Clients;
using Acceptance.Tests.Helpers;
using Acceptance.Tests.Models;
using Acceptance.Tests.TestData;
using RestSharp;
using Reqnroll;

namespace Acceptance.Tests.StepDefinitions;

[Binding]
public class SimActivationSteps
{
    private readonly ActivationApiClient _activationApiClient;
    private ActivationRequest _activationRequest = null!;
    private RestResponse _response = null!;

    public SimActivationSteps()
    {
        var baseUrl = ConfigurationHelper.GetBaseUrl();
        var restClient = new RestClient(baseUrl);

        _activationApiClient = new ActivationApiClient(restClient);
    }

    [Given("I have a valid SIM activation request")]
    public void GivenIHaveAValidSimActivationRequest()
    {
        _activationRequest = ActivationTestData.ValidActivationRequest();
    }

    [Given("I have an activation request without {string}")]
    public void GivenIHaveAnActivationRequestWithout(string missingField)
    {
        _activationRequest = missingField switch
        {
            "iccid" => ActivationTestData.RequestWithoutIccid(),
            "customerId" => ActivationTestData.RequestWithoutCustomerId(),
            "planCode" => ActivationTestData.RequestWithoutPlanCode(),
            _=> throw new ArgumentException($"Unsupported missing field: {missingField}")
        };
    }

    [When("I send the activation request")]
    public async Task WhenISendTheActivationRequest()
    {
        _response = await _activationApiClient.ActivateSimAsync(_activationRequest);
    }

    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
    {
        Assert.That((int)_response.StatusCode, Is.EqualTo(expectedStatusCode));
    }

    [Then("the activation status should be {string}")]
    public void ThenTheActivationStatusShouldBe(string expectedStatus)
    {
        var body = JsonHelper.Deserialize<ActivationResponse>(_response.Content!);

        Assert.That(body.Status, Is.EqualTo(expectedStatus));
    }

    [Then("the error message should be {string}")]
    public void ThenTheErrorMessageShouldBe(string expectedErrorMessage)
    {
        var body = JsonHelper.Deserialize<ErrorResponse>(_response.Content!);

        Assert.That(body.Error, Is.EqualTo(expectedErrorMessage));
    }

}