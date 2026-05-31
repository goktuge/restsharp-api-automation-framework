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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain("Healthy"));
        }

    }

    [Test]
    public async Task Should_accept_valid_sim_activation_request()
    {
        var activationRequest = ActivationTestData.ValidActivationRequest();

        var response = await ActivationApiClient.ActivateSimTypedAsync(activationRequest);
        var responseBody = response.Body;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
            Assert.That(response, Is.Not.Null);
            Assert.That(responseBody!.Status, Is.EqualTo("Accepted"));
            Assert.That(responseBody.Iccid, Is.EqualTo(activationRequest.Iccid));
            Assert.That(responseBody.CustomerId, Is.EqualTo(activationRequest.CustomerId));
            Assert.That(responseBody.ActivationId, Is.Not.Empty);
        }

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

        var errorResponse = ApiResponseMapper.ToApiResponse<ErrorResponse>(response);

        Assert.That(errorResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(errorResponse.Body!.Error, Is.EqualTo(expectedErrorMessage));
    }

    [Test]
    public async Task Should_return_correlation_id_when_header_is_provided()
    {
        var activationRequest = ActivationTestData.ValidActivationRequest();
        var correlationId = Guid.NewGuid().ToString();

        var response = await ActivationApiClient.ActivateSimTypedAsync(
            activationRequest,
            token: "test-token",
            correlationId: correlationId
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
        Assert.That(response.Body!.CorrelationId, Is.EqualTo(correlationId));

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

    [Test]
    public async Task Should_get_activation_by_id_after_successful_activation()
    {
        var (activationRequest, createdActivation) = await CreateValidActivationAsync();

        var getResponse = await ActivationApiClient
         .GetActivationByIdTypedAsync<ActivationResponse>(createdActivation.ActivationId);

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var fetchedActivation = getResponse.Body!;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fetchedActivation.ActivationId, Is.EqualTo(createdActivation.ActivationId));
            Assert.That(fetchedActivation.Iccid, Is.EqualTo(activationRequest.Iccid));
            Assert.That(fetchedActivation.CustomerId, Is.EqualTo(activationRequest.CustomerId));
            Assert.That(fetchedActivation.Status, Is.EqualTo("Accepted"));
        }
    }

    [Test]
    public async Task Should_return_not_found_when_activation_id_does_not_exist()
    {
        var unknownActivationId = Guid.NewGuid().ToString();

        var response = await ActivationApiClient.GetActivationByIdTypedAsync<ErrorResponse>(unknownActivationId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(response.Body!.Error, Is.EqualTo("Activation not found"));

    }

    [Test]
    public async Task Should_filter_activations_by_customer_id()
    {
        var customerId = $"customer-{Guid.NewGuid()}";

        var activationRequest = ActivationTestData.ValidActivationRequest() with
        {
            CustomerId = customerId
        };

        var createResponse = await ActivationApiClient.ActivateSimTypedAsync(activationRequest);

        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));

        var listResponse = await ActivationApiClient
        .GetActivationsTypedAsync<List<ActivationResponse>>(customerId: customerId);
        var activations = listResponse.Body!;

        Assert.That(listResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(activations, Is.Not.Empty);

        foreach (var activation in activations)
        {
            Assert.That(activation.CustomerId, Is.EqualTo(customerId));
        }

        Assert.That(
            activations.Any(activation => activation.ActivationId == createResponse.Body!.ActivationId),
            Is.True
        );
    }

    [Test]
    public async Task Should_filter_activations_by_status()
    {
        var activationRequest = ActivationTestData.ValidActivationRequest();

        var createResponse = await ActivationApiClient.ActivateSimAsync(activationRequest);

        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));

        var listResponse = await ActivationApiClient
        .GetActivationsTypedAsync<List<ActivationResponse>>(status: "Accepted");

        Assert.That(listResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var activations = listResponse.Body!;

        Assert.That(activations, Is.Not.Empty);

        foreach (var activation in activations)
        {
            Assert.That(activation.Status, Is.EqualTo("Accepted"));
        }
    }

    [Test]
    public async Task Should_update_activation_status()
    {
        var (activationRequest, createdActivation) = await CreateValidActivationAsync();

        var updateRequest = new UpdateActivationStatusRequest(
             Status: "Completed"
         );

        var updateResponse = await ActivationApiClient.UpdateActivationStatusTypedAsync<ActivationResponse>(
        createdActivation.ActivationId,
        updateRequest);

        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var updatedActivation = updateResponse.Body!;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedActivation.ActivationId, Is.EqualTo(createdActivation.ActivationId));
            Assert.That(updatedActivation.Status, Is.EqualTo("Completed"));
            Assert.That(updatedActivation.Iccid, Is.EqualTo(activationRequest.Iccid));
            Assert.That(updatedActivation.CustomerId, Is.EqualTo(activationRequest.CustomerId));
        }
    }

    [Test]
    public async Task Should_return_not_found_when_updating_unknown_activation()
    {
        var unknownActivationId = Guid.NewGuid().ToString();

        var updateRequest = new UpdateActivationStatusRequest(
            Status: "Completed"
        );

        var response = await ActivationApiClient.UpdateActivationStatusTypedAsync<ErrorResponse>(
            unknownActivationId,
            updateRequest
        );

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var body = response.Body;

        Assert.That(body!.Error, Is.EqualTo("Activation not found"));
    }

    [Test]
    public async Task Should_delete_activation_and_return_not_found_afterwards()
    {
        var (_, createdActivation) = await CreateValidActivationAsync();

        var deleteResponse = await ActivationApiClient.DeleteActivationAsync(createdActivation.ActivationId);

        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var getResponse = await ActivationApiClient.GetActivationByIdAsync(createdActivation.ActivationId);

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}




