using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Attributes;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Memory;
using Elsa.Workflows.Models;
using Elsa.Workflows.UIHints;
using System.ComponentModel;

namespace ElsaServer.BotPro.Mensajeria;

[FlowNode(CONTINUAR)]
[Category(" BOTPRO MENSAJES")]
public class EnviarMensaje: Activity
{
    const string CONTINUAR = "Continuar";

    [Input(UIHint = InputUIHints.MultiLine, DisplayName = "Contenido del mensaje", Description = "Aquí va el contenido del mensaje!")]
    public Input<string>? Mensaje { get; set; }

    [Input(DisplayName = "Esperar respuesta", Description ="Al momento de esperar respuesta su flujo se pausará hasta que se envíe un nuevo mensaje de parte del usuario")]
    public bool EsperarRespuesta { get; set; }

    [Input(UIHint = InputUIHints.VariablePicker, DisplayName ="Variable respuesta",  Description = "Escoja la variable donde será guardada la respues del usuario!")]
    public Variable ContenidoRespuesta { get; set; }


    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        await base.ExecuteAsync(context);

        if (context.IsTriggerOfWorkflow())
        {
            // Mandar el mensaje
            Console.WriteLine($"\n\n\n\nMensaje: {Mensaje}\n\n\n\n");

            if (EsperarRespuesta)
            {
                var pausa = new CreateBookmarkArgs();
                pausa.BookmarkName = "EsperarRespuesta";
                pausa.Callback = ResumirAsync;
                pausa.IncludeActivityInstanceId = true;

                var actividadId = context.Activity.Id;
                Console.WriteLine($"\n\n\n\nActividadId: {actividadId}\n\n\n\n");
                context.CreateBookmark(pausa);
            }
            else
                await context.CompleteActivityWithOutcomesAsync(CONTINUAR);
        }
        else
        {
            Console.WriteLine($"\n\n\n\n NUNCA DEBE ENTRAR AQUI \n\n\n\n");
            await context.CompleteActivityWithOutcomesAsync(CONTINUAR);
        }

    }

    private async ValueTask ResumirAsync(ActivityExecutionContext context)
    {
        Console.WriteLine($"\n\n\nFlujo continuado: \n\n\n\n");
        await context.CompleteActivityWithOutcomesAsync(CONTINUAR);
    }
}
