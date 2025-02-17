using System.ComponentModel;
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;

namespace Elsa.Api.Activities;

[Activity(" BotPro", "Imprimir Mensaje")]
public class PrintMessage : CodeActivity
{
    [Input(Description = "Texto del mensaje a imprimir.")]
    public required Input<string> Message { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        var message = Message.Get(context);
        Console.WriteLine(message);
    }
}