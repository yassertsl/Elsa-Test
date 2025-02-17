using Elsa.Api.Activities;
using Elsa.Workflows.Activities;
using Elsa.Workflows;

namespace Elsa.Api.Workflows;

public class MyEventWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new Sequence
        {
            Activities =
           {
               new WriteLine("Starting workflow..."),
               new MyEvent(), // This will block further execution until the MyEvent's bookmark is resumed.
               new WriteLine("Event occurred!")
           }
        };
    }
}