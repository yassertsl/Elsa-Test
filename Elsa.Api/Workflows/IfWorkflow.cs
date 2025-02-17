﻿using Elsa.Workflows.Activities;
using Elsa.Workflows;

namespace Elsa.Api.Workflows;

public class IfWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Root = new If
        {
            Condition = new(context => DateTime.Now.IsDaylightSavingTime()),
            Then = new WriteLine("Welcome to the light side!"),
            Else = new WriteLine("Welcome to the dark side!")
        };
    }
}