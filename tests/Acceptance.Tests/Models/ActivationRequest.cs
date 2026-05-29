namespace Acceptance.Tests.Models
{

    public record ActivationRequest(
        string Iccid,
        string CustomerId,
        string PlanCode
    );

    public record ActivationResponse(
        string Iccid,
        string CustomerId,
        string ActivationId,
        string PlanCode,
        string Status,
        DateTimeOffset CreatedAtUtc
    );

    public record ErrorResponse(
        string Error
    );
}