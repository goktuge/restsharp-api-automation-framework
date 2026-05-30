var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

var activations = new Dictionary<string, ActivateSimResponse>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        serviceName = "Activation.Api"
    });
});

app.MapPost("/activations", (ActivateSimRequest request, HttpRequest httpRequest) =>
{
    var authorizationHeader = httpRequest.Headers["Authorization"].FirstOrDefault();

    if (authorizationHeader != "Bearer test-token")
    {
        return Results.Unauthorized();
    }

    var correlationId = httpRequest.Headers["X-Correlation-Id"].FirstOrDefault()
        ?? Guid.NewGuid().ToString();

    if (string.IsNullOrWhiteSpace(request.Iccid))
    {
        return Results.BadRequest(new
        {
            error = "ICCID is required"
        });
    }

    if (string.IsNullOrWhiteSpace(request.CustomerId))
    {
        return Results.BadRequest(new
        {
            error = "CustomerId is required"
        });
    }

    if (string.IsNullOrWhiteSpace(request.PlanCode))
    {
        return Results.BadRequest(new
        {
            error = "PlanCode is required"
        });
    }

    var response = new ActivateSimResponse(
        ActivationId: Guid.NewGuid().ToString(),
        Iccid: request.Iccid,
        CustomerId: request.CustomerId,
        Status: "Accepted",
        CreatedAtUtc: DateTimeOffset.UtcNow,
        CorrelationId: correlationId
    );

    activations[response.ActivationId] = response;

    return Results.Accepted($"/activations/{response.ActivationId}", response);
});

app.MapGet("/activations/{activationId}", (string activationId, HttpRequest httpRequest) =>
{
    var authorizationHeader = httpRequest.Headers["Authorization"].FirstOrDefault();

    if (authorizationHeader != "Bearer test-token")
    {
        return Results.Unauthorized();
    }

    if (!activations.TryGetValue(activationId, out var activation))
    {
        return Results.NotFound(new
        {
            error = "Activation not found"
        });
    }

    return Results.Ok(activation);
});

app.MapGet("/activations", (
    HttpRequest httpRequest,
    string? status,
    string? customerId) =>
{
    var authorizationHeader = httpRequest.Headers["Authorization"].FirstOrDefault();

    if (authorizationHeader != "Bearer test-token")
    {
        return Results.Unauthorized();
    }

    IEnumerable<ActivateSimResponse> results = activations.Values;

    if (!string.IsNullOrWhiteSpace(status))
    {
        results = results.Where(activation =>
            string.Equals(activation.Status, status, StringComparison.OrdinalIgnoreCase));
    }

    if (!string.IsNullOrWhiteSpace(customerId))
    {
        results = results.Where(activation =>
            activation.CustomerId == customerId);
    }

    return Results.Ok(results.ToList());
});

app.Run();

public record ActivateSimRequest(
    string Iccid,
    string CustomerId,
    string PlanCode
);

public record ActivateSimResponse(
    string ActivationId,
    string Iccid,
    string CustomerId,
    string Status,
    string CorrelationId,
    DateTimeOffset CreatedAtUtc
);