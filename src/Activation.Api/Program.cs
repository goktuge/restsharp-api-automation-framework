var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

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

app.MapPost("/activations", (ActivateSimRequest request) =>
{
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
        CreatedAtUtc: DateTimeOffset.UtcNow
    );

    return Results.Accepted($"/activations/{response.ActivationId}", response);
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
    DateTimeOffset CreatedAtUtc
);