using System.Net;
using Acceptance.Tests.Models;
using Acceptance.Tests.Clients;
using Acceptance.Tests.TestData;
using Acceptance.Tests.Helpers;
using Acceptance.Tests.Base;
using NUnit.Framework;

namespace Acceptance.Tests;

public class ActivationApiTests : ApiTestBase
{

    private static IEnumerable<TestCaseData> InvalidActivationRequests()
    {
        yield return new TestCaseData(
            ActivationTestData.RequestWithoutIccid(),
            "ICCID is required"
        ).SetName("Should reject activation request without ICCID");

        yield return new TestCaseData(
            ActivationTestData.RequestWithoutCustomerId(),
            "CustomerId is required"
        ).SetName("Should reject activation request without CustomerId");

        yield return new TestCaseData(
            ActivationTestData.RequestWithoutPlanCode(),
            "PlanCode is required"
        ).SetName("Should reject activation request without PlanCode");
    }

    [Test]
    public async Task Should_return_health_status()
    {
        var response = await ActivationApiClient.GetHealthAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content, Does.Contain("Healthy"));
    }

    [Test]
    public async Task Should_accept_valid_sim_activation_request()
    {
        var activationRequest = ActivationTestData.ValidActivationRequest();

        var response = await ActivationApiClient.ActivateSimAsync(activationRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));

        var body = JsonHelper.Deserialize<ActivationResponse>(response.Content!);

        Assert.That(body, Is.Not.Null);
        Assert.That(body.Status, Is.EqualTo("Accepted"));
        Assert.That(body.Iccid, Is.EqualTo(activationRequest.Iccid));
        Assert.That(body.CustomerId, Is.EqualTo(activationRequest.CustomerId));
        Assert.That(body.ActivationId, Is.Not.Empty);
    }

    // [Test]
    // public async Task Should_reject_activation_request_without_iccid()
    // {
    //     var request = ActivationTestData.RequestWithoutIccid();

    //     var response = await ActivationApiClient.ActivateSimAsync(request);

    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    //     var body = JsonHelper.Deserialize<ErrorResponse>(response.Content!);

    //     Assert.That(body, Is.Not.Null);
    //     Assert.That(body.Error, Is.EqualTo("ICCID is required"));
    // }

    // [Test]
    // public async Task Should_reject_activation_request_without_customer_id()
    // {

    //     var ActivationRequest = ActivationTestData.RequestWithoutCustomerId();
    //     var response = await ActivationApiClient.ActivateSimAsync(ActivationRequest);

    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    //     var body = JsonHelper.Deserialize<ErrorResponse>(response.Content!);

    //     Assert.That(body.Error, Is.EqualTo("CustomerId is required"));
    // }


    // [Test]
    // public async Task Should_reject_activation_request_without_plan_code()
    // {
    //     var activationRequest = ActivationTestData.RequestWithoutPlanCode();

    //     var response = await ActivationApiClient.ActivateSimAsync(activationRequest);

    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

    //     var body = JsonHelper.Deserialize<ErrorResponse>(response.Content!);

    //     Assert.That(body.Error, Is.EqualTo("PlanCode is required"));
    // }

    [TestCaseSource(nameof(InvalidActivationRequests))]
    public async Task Should_reject_invalid_activation_requests(
        ActivationRequest activationRequest,
        string expectedErrorMessage)
    {
        var response = await ActivationApiClient.ActivateSimAsync(activationRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var body = JsonHelper.Deserialize<ErrorResponse>(response.Content!);

        Assert.That(body.Error, Is.EqualTo(expectedErrorMessage));
    }

    [Test]
    public async Task Should_return_correlation_id_when_header_is_provided()
    {
        var activationRequest = ActivationTestData.ValidActivationRequest();
        var correlationId = Guid.NewGuid().ToString();

        var response = await ActivationApiClient.ActivateSimAsync(
            activationRequest,
            token: "test-token",
            correlationId: correlationId
        );
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));

        var body = JsonHelper.Deserialize<ActivationResponse>(response.Content!);

        Assert.That(body.CorrelationId, Is.EqualTo(correlationId));
    }

    [Test]
    public async Task Should_reject_activation_request_without_authorization_token()
    {
        var activationRequest = ActivationTestData.ValidActivationRequest();

        var response = await ActivationApiClient.ActivateSimAsync(
            activationRequest,
            token: null
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Should_reject_activation_request_with_invalid_token()
    {
        var activationRequest = ActivationTestData.ValidActivationRequest();

        var response = await ActivationApiClient.ActivateSimAsync(
            activationRequest,
            token: "wrong-token"
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}




