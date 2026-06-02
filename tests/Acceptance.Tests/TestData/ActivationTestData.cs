using Acceptance.Tests.Models;

namespace Acceptance.Tests.TestData;

public static class ActivationTestData
{
    public static ActivationRequest ValidActivationRequest()
    {
        return new ActivationRequest(
            Iccid: $"customer-{Guid.NewGuid():N}",
            CustomerId: "customer-001",
            PlanCode: "EU_5GB"
        );
    }

    public static ActivationRequest RequestWithoutIccid()
    {
        return new ActivationRequest(
            Iccid: "",
            CustomerId: "customer-001",
            PlanCode: "EU_5GB"
        );
    }

    public static ActivationRequest RequestWithoutCustomerId()
    {
        return new ActivationRequest(
            Iccid: $"customer-{Guid.NewGuid():N}",
            CustomerId: "",
            PlanCode: "EU_5GB"
        );
    }

    public static ActivationRequest RequestWithoutPlanCode()
    {
        return new ActivationRequest(
            Iccid: $"customer-{Guid.NewGuid():N}",
            CustomerId: "customer-001",
            PlanCode: ""
        );
    }
}