using Elsa.Workflows.Models;
using Elsa.Workflows;
using Elsa.Extensions;
using System.ComponentModel;
using Elsa.Workflows.Attributes;

namespace Elsa.Api.Activities;

[Category(" BotPro")]
public class GenerateRandomNumber : CodeActivity
{
    [Output(Description = "Numero generado aleatorio.")]
    public Output<decimal> Result { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        var randomNumber = Random.Shared.Next(1, 100);
        Result.Set(context, randomNumber);
    }
}