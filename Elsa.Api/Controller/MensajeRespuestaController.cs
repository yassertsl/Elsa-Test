using Elsa.Api.Activities;
using Elsa.Api.Entity;
using Elsa.Extensions;
using Elsa.Workflows.Helpers;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Filters;
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
    public class MensajeRespuestaController(IWorkflowRuntime workflowRuntime) : ControllerBase
    {
        private readonly IWorkflowRuntime _workflowRuntime = workflowRuntime;

        [HttpPost]
        public async Task<IActionResult> Resume([FromBody] ResumeWorkflowRequest request, CancellationToken cancellationToken = default)
        {
            var _client = await _workflowRuntime.CreateClientAsync(request.WorkflowInstanceId, cancellationToken);
            Console.WriteLine($"\n\n\n\nResumeWorkflowRequest: {_client.WorkflowInstanceId}\n\n\n\n");

            await _client.RunInstanceAsync(new RunWorkflowInstanceRequest
            {
                TriggerActivityId = request.ActivityId,
                Input = request.Input,
                BookmarkId = request.BookmarkId,
                ActivityHandle = new ActivityHandle(),
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
