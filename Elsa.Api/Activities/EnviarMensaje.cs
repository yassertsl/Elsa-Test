using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Activities.Flowchart.Attributes;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Management.Models;
using Elsa.Workflows.Memory;
using Elsa.Workflows.Models;
using Elsa.Workflows.UIHints;
using System.ComponentModel;
using System.Threading;

namespace Elsa.Api.Activities;

[FlowNode(CONTINUAR)]
[Category(" BOTPRO MENSAJES")]
public class EnviarMensaje : Activity
{
    private const string CONTINUAR = "Continuar";

    [Input(UIHint = InputUIHints.MultiLine, DisplayName = "Contenido del mensaje", Description = "Aquí va el contenido del mensaje!")]
    public Input<string>? Mensaje { get; set; }

    [Input(DisplayName = "Esperar respuesta", Description = "Al momento de esperar respuesta, su flujo se pausará hasta que se envíe un nuevo mensaje de parte del usuario")]
    public bool EsperarRespuesta { get; set; }

    [Input(UIHint = InputUIHints.VariablePicker, DisplayName = "Variable respuesta", Description = "Escoja la variable donde será guardada la respuesta del usuario!")]
    public Variable<string> ContenidoRespuesta { get; set; }

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        await base.ExecuteAsync(context);

        if (!context.IsTriggerOfWorkflow())
        {
            // Log the message
            Console.WriteLine($"\n\n\n\nMensaje: {Mensaje}\n\n\n\n");

            if (EsperarRespuesta)
            {
                Console.WriteLine("\n\n\nWorkflow suspended waiting for response...\n\n\n");

                // Create a bookmark to suspend execution
                context.CreateBookmark(new CreateBookmarkArgs
                {
                    BookmarkName = "EsperarRespuesta",
                    Callback = ResumirAsync,
                    IncludeActivityInstanceId = true
                });

                return;
            }

            Console.WriteLine($"\n\n No esperar respuesta : \n\n\n");

            // Continue execution if no waiting is required
            await context.CompleteActivityWithOutcomesAsync(CONTINUAR);
        }
        else
        {
            Console.WriteLine($"\n\n\n\n ERROR: Workflow triggered incorrectly \n\n\n\n");
            await context.CompleteActivityWithOutcomesAsync(CONTINUAR);
        }
    }

    private async ValueTask ResumirAsync(ActivityExecutionContext context)
    {
        if (context.TryGetWorkflowInput<string>("answer", out var answer))
        {
            Console.WriteLine($"\n\n\nReceived answer: {answer}\n\n\n");

            // Store the answer in the workflow variable
            context.SetVariable(ContenidoRespuesta.Name, answer);
        }
        else
        {
            Console.WriteLine($"\n\n\nERROR: No answer found in workflow input!\n\n\n");
        }

        // Resume execution
        await context.CompleteActivityWithOutcomesAsync(CONTINUAR);
    }

}