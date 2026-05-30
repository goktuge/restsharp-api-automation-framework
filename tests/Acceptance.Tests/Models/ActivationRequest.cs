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
        string CorrelationId,
        DateTimeOffset CreatedAtUtc
    );

    public record ErrorResponse(
        string Error
    );

    public record UpdateActivationStatusRequest(
    string Status
);
}