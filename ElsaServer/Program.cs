using AspNetCore.Authentication.ApiKey;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Identity.Providers;
using Elsa.Workflows;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Contracts;
using Elsa.Workflows.Runtime.Requests;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite()));
    elsa.UseIdentity(identity =>
    {
        identity.TokenOptions = options => options.SigningKey = "sufficiently-large-secret-signing-key"; // This key needs to be at least 256 bits long.
        identity.UseAdminUserProvider();
    });

    var config = builder.Configuration;
    var adminApiKey = builder.Configuration["Elsa:Authentication:AdminApiKey"];

    if (string.IsNullOrEmpty(adminApiKey))
    {
        throw new InvalidOperationException("Missing Admin API Key in appsettings.json!");
    }

    Console.WriteLine($"Loaded Admin API Key: {adminApiKey}");

    // Manually inject the API key to ensure Elsa uses it
    builder.Services.Configure<Elsa.Identity.Options.IdentityTokenOptions>(options =>
    {
        options.SigningKey = adminApiKey;
    });

    // Ensure Elsa API key provider receives the correct key
    builder.Services.Configure<ApiKeyOptions>(options =>
    {
        options.IgnoreAuthenticationIfAllowAnonymous = true;
    });

    builder.Services.AddScoped<IApiKeyProvider, AdminApiKeyProvider>();

    elsa.UseDefaultAuthentication(auth =>
    {
        auth.UseAdminApiKey();
    });



    elsa.UseWorkflowsApi();
    elsa.UseJavaScript();
    elsa.UseCSharp();
    elsa.UseLiquid();
    elsa.UseHttp();
    elsa.UseScheduling();
    elsa.AddActivitiesFrom<Program>();
    elsa.AddWorkflowsFrom<Program>();
    elsa.AddSwagger();
});

builder.Services.AddCors(cors => cors
    .AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("x-elsa-workflow-instance-id"))); // Required for Elsa Studio in order to support running workflows from the designer. Alternatively, you can use the `*` wildcard to expose all headers.



builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwaggerGen();
app.UseSwaggerUI();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi(); // Use Elsa API endpoints.
app.UseWorkflows(); // Use Elsa middleware to handle HTTP requests mapped to HTTP Endpoint activities.


app.MapPost("/resume", async (
    [FromServices] IWorkflowDispatcher dispatcher,
    [FromServices] IWorkflowInstanceStore instanceStore,
    [FromServices] ILogger<Program> logger,
    [FromBody] ResumeWorkflowRequest request) =>
{
    try
    {
        // Validate workflow instance
        var instance = await instanceStore.FindAsync(new WorkflowInstanceFilter { Id = request.WorkflowInstanceId });
        if (instance == null)
            return Results.NotFound("Workflow instance not found");

        // Validate bookmark
        var bookmark = instance.WorkflowState.Bookmarks.FirstOrDefault(b => b.Id == request.BookmarkId);
        if (bookmark == null)
            return Results.NotFound("Bookmark not found or already consumed");

        logger.LogInformation(
            "Found bookmark {BookmarkName} with ID {BookmarkId}",
            bookmark.Name,
            bookmark.Id);

        // For this specific activity type (EsperarRespuesta), we need to format the input correctly
        // The input should be the string value for "Nombre del usuario"
        var dispatchRequest = new DispatchTriggerWorkflowsRequest(
            "EsperarRespuesta",  // Use the bookmark name as the activity type
            request.Input)
        {
            WorkflowInstanceId = request.WorkflowInstanceId,
            ActivityInstanceId = bookmark.ActivityInstanceId
        };

        logger.LogInformation(
            "Dispatching trigger for workflow {Id} with input {Input}",
            request.WorkflowInstanceId,
            System.Text.Json.JsonSerializer.Serialize(request.Input));

        var result = await dispatcher.DispatchAsync(dispatchRequest);

        if (result.Succeeded)
        {
            logger.LogInformation(
                "Successfully triggered workflow {Id}",
                request.WorkflowInstanceId);
            return Results.Ok(new
            {
                Status = "Triggered Successfully",
                WorkflowInstanceId = request.WorkflowInstanceId,
                BookmarkId = request.BookmarkId
            });
        }

        logger.LogError(
            "Failed to trigger workflow {Id}",
            request.WorkflowInstanceId);
        return Results.BadRequest("Failed to trigger workflow.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing workflow resume request");
        return Results.BadRequest(new { Error = ex.Message, Details = ex.ToString() });
    }
});


app.Run();



public class ResumeWorkflowRequest
{
    public required string WorkflowInstanceId { get; set; }
    public required string BookmarkId { get; set; }
    public IDictionary<string, object>? Input { get; set; } = new Dictionary<string, object>();
}
