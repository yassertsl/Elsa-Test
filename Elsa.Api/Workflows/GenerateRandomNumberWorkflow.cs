using Elsa.Api.Activities;
using Elsa.Workflows;
using Elsa.Workflows.Activities;

namespace Elsa.Api.Workflows;

public class GenerateRandomNumberWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        var randomNumber = builder.WithVariable("RandomNumber", 0m);

        builder.Root = new Sequence
        {
            Activities =
            {
                new GenerateRandomNumber
                {
                    Result = new(randomNumber)
                },
                new PrintMessage
                {
                    Message = new(context => $"The random number is: {randomNumber.Get(context)}")
                }
            }
        };
    }
}