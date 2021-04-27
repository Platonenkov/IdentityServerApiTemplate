using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IdentityServerApiTemplate.Server.AppStart.SwaggerFilters
{
    /// <summary>
    /// Swagger Method Info Generator from summary for <see cref="M:GetPaged{T}"/>
    /// </summary>
    public class ApplySummariesOperationFilter : IOperationFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor controller_action_descriptor))
            {
                return;
            }

            var action_name = controller_action_descriptor.ActionName;
            if (action_name != "GetPaged")
            {
                return;
            }

            var resource_name = controller_action_descriptor.ControllerName;
            operation.Summary = $"Returns paged list of the {resource_name} as IPagedList wrapped with OperationResult";
        }
    }
}