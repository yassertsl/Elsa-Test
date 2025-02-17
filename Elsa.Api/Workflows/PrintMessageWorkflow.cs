using Elsa.Api.Activities;
using Elsa.Workflows;

namespace Elsa.Api.Workflows;

public class PrintMessageWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var message = builder.WithVariable("Message", "Hello, World!");

        builder.Root = new PrintMessage
        {
            Message = new(context => $"The message is: {message.Get(context)}")
        };
    }
}