using Elsa.Api.Activities;
using Elsa.Api.Entity;
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Helpers;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Compression;
using Elsa.Workflows.Management.Entities;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Management.Models;
using Elsa.Workflows.Memory;
using Elsa.Workflows.Models;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Messages;
using Elsa.Workflows.Runtime.Parameters;
using Elsa.Workflows.Runtime.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Elsa.Api.Controller
{
    [ApiController]
    [Route("api/MensajeRespuesta")]
    public class MensajeRespuestaController(IWorkflowRuntime workflowRuntime, IWorkflowInstanceVariableManager workflowInstanceVariableManager) : ControllerBase
    {
        private readonly IWorkflowRuntime _workflowRuntime = workflowRuntime;
        private readonly IWorkflowInstanceVariableManager _workflowInstanceVariableManager = workflowInstanceVariableManager;

        [HttpPost]
        public async Task<IActionResult> Resume([FromBody] ResumeWorkflowRequest request, CancellationToken cancellationToken = default)
        {
            var _client = await _workflowRuntime.CreateClientAsync(request.WorkflowInstanceId, cancellationToken);
            Console.WriteLine($"\n\n\n\nResumeWorkflowRequest: {_client.WorkflowInstanceId}\n\n\n\n");

            var variables = await _workflowInstanceVariableManager.GetVariablesAsync(request.WorkflowInstanceId, cancellationToken: cancellationToken);

            var variableUpdateValues = new List<VariableUpdateValue>();
            foreach (var variable in variables)
            {
                if (request.Input != null && request.Input.TryGetValue(variable.Variable.Name, out var nombreUsuario))
                {
                    variable.Variable.Value = nombreUsuario;
                    variableUpdateValues.Add(new VariableUpdateValue(variable.Variable.Id, variable.Variable.Value));
                }
                Console.WriteLine($"Id: {variable.Variable.Id},  Name: {variable.Variable.Name}, Value: {variable.Variable.Value}");
            }
            await _workflowInstanceVariableManager.SetVariablesAsync(request.WorkflowInstanceId, variableUpdateValues, cancellationToken);

            await _client.RunInstanceAsync(new RunWorkflowInstanceRequest
            {
                TriggerActivityId = request.ActivityId,
                Input = request.Input,
                BookmarkId = request.BookmarkId,
                ActivityHandle = new ActivityHandle()
            }, cancellationToken);
            return Ok(new
            {
                Status = "Triggered Successfully",
                WorkflowInstanceId = request.WorkflowInstanceId,
                BookmarkId = request.BookmarkId
            });
        }
    }
}
