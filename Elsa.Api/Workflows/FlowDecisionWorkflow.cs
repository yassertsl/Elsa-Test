﻿using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Activities.Flowchart.Activities;
using Endpoint = Elsa.Workflows.Activities.Flowchart.Models.Endpoint;

namespace Elsa.Api.Workflows;

public class FlowDecisionWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Name = "Flow Decision Workflow";

        var age = builder.WithVariable<int>();
        var promptWriteLine = new WriteLine("What is your age?");
        var readLine = new ReadLine(age);
        var isAdult = new FlowDecision(context => age.Get(context) >= 18);
        var adult = new WriteLine("You are an adult.");
        var minor = new WriteLine("You are a minor.");
        var finish = new Finish();

        builder.Root = new Flowchart
        {
            Activities =
            {
                promptWriteLine,
                readLine,
                isAdult,
                adult,
                minor,
                finish
            },
            Connections =
            {
                new(promptWriteLine, readLine),
                new(readLine, isAdult),
                new(new Endpoint(isAdult, "True"), new Endpoint(adult)),
                new(new Endpoint(isAdult, "False"), new Endpoint(minor)),
                new(new Endpoint(adult), new Endpoint(finish)),
                new(new Endpoint(minor), new Endpoint(finish))
            }
        };
    }
}