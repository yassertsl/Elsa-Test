namespace Elsa.Api.Entity;

public class ResumeWorkflowRequest
{
    public required string WorkflowInstanceId { get; set; }
    public required string BookmarkId { get; set; }
    public required string ActivityId { get; set; }
    public IDictionary<string, object>? Input { get; set; } = new Dictionary<string, object>();
}
