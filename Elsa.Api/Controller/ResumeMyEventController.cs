using Elsa.Api.Activities;
using Elsa.Extensions;
using Elsa.Workflows.Helpers;
using Elsa.Workflows.Runtime;
using Microsoft.AspNetCore.Mvc;

namespace Elsa.Api.Controller
{
    [ApiController]
    [Route("api/MyEvent")]
    public class MyEventController(IWorkflowRuntime workflowRuntime) : ControllerBase
    {
        private readonly IWorkflowRuntime _workflowRuntime = workflowRuntime;

        [HttpGet("Resume")]
        public async Task <IActionResult> Resume()
        {
            var bookmarkPayload = "MyEvent";
            var activityTypeName = ActivityTypeNameHelper.GenerateTypeName<MyEvent>();
            await _workflowRuntime.TriggerWorkflowsAsync(activityTypeName, bookmarkPayload);
            return Ok();
        }

        [HttpGet("ResumeById/{instanceId}")]
        public async Task<IActionResult> ResumeById(string instanceId, CancellationToken cancellationToken = default)
        {
            var bookmarkPayload = "MyEvent";
            var activityTypeName = ActivityTypeNameHelper.GenerateTypeName<MyEvent>();
            await _workflowRuntime.ResumeWorkflowAsync(instanceId);
            await _workflowRuntime.ResumeWorkflowsAsync(activityTypeName, bookmarkPayload);
            var _client = await _workflowRuntime.CreateClientAsync(instanceId, cancellationToken);
            return Ok();
        }
    }
}
