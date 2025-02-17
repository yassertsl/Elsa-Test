using Elsa.Workflows.Activities.Flowchart.Attributes;
using Elsa.Workflows;
using System.ComponentModel;

namespace Elsa.Api.Activities;

[Category(" BotPro")]
[FlowNode("Pass", "Fail")]
public class PerformTask : Activity
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        await context.CompleteActivityWithOutcomesAsync("Pass");
    }
}