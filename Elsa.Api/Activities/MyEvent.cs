using System.ComponentModel;
using Elsa.Extensions;
using Elsa.Workflows;

namespace Elsa.Api.Activities;

[Category(" BotPro")]
public class MyEvent : Trigger
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        if (context.IsTriggerOfWorkflow())
        {
            await context.CompleteActivityAsync();
            return;
        }

        context.CreateBookmark("MyEvent");
    }

    protected override object GetTriggerPayload(TriggerIndexingContext context)
    {
        return "MyEvent";
    }
}