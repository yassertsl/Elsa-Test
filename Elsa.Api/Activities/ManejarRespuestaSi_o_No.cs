using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Attributes;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Memory;
using Elsa.Workflows.Models;
using Elsa.Workflows.UIHints;
using System.ComponentModel;

namespace Elsa.Api.Activities;

[FlowNode(SI, NO, OTRA_COSA)]
[Category(" BOTPRO MENSAJES")]
[DisplayName("Manejar respuesta Si o No")]
public class ManejarRespuestaSi_o_No : Activity
{
    const string SI = "SI";
    const string NO = "NO";
    const string OTRA_COSA = "OTRA COSA";

    [Input(UIHint = InputUIHints.VariablePicker, DisplayName = "Variable respuesta", Description = "Seleccione la variable donde será guardada la respuesta del usuario!")]
    public required Variable<string> ContenidoRespuesta { get; set; } 

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        await base.ExecuteAsync(context);

        if (ContenidoRespuesta != null)
        {
            var variableName = ContenidoRespuesta?.Name ?? "RespuestaUsuario";
            var valor = context.GetVariable<string>(variableName) ?? "";

            //var valor = context.GetVariable<string>(ContenidoRespuesta.Name ?? "RespuestaUsuario") ?? "";
            if (string.IsNullOrEmpty(valor))
            {
                Console.WriteLine($"\n\n\n\n VALOR DE LA VARIABLE ES NULO \n\n\n\n");
                await context.CompleteActivityWithOutcomesAsync(OTRA_COSA);
            }

            if (valor.ToLower() == "si")
            {
                Console.WriteLine($"\n\n\n\n VALOR SI \n\n\n\n");
                await context.CompleteActivityWithOutcomesAsync(SI);
            }
            else if (valor.ToLower() == "no")
            {
                Console.WriteLine($"\n\n\n\n VALOR NO \n\n\n\n");
                await context.CompleteActivityWithOutcomesAsync(NO);
            }
            else
            {
                Console.WriteLine($"\n\n\n\n VALOR OTRA COSA: {valor} \n\n\n\n");
                await context.CompleteActivityWithOutcomesAsync(OTRA_COSA);
            }
        }
        else
        {
            Console.WriteLine($"\n\n\n\n CONTENIDO RESPUESTA ES NULO \n\n\n\n");
            await context.CompleteActivityWithOutcomesAsync(OTRA_COSA);
        }

    }
}