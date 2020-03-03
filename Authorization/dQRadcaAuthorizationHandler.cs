using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

using dQRadca.Models;

namespace dQRadca.Authorization
{
    public class dQRadcaAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Product>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Product resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Resources.ApplicationTexts.ApproveOperationName
                && requirement.Name != Resources.ApplicationTexts.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Resources.ApplicationTexts.ProductAccountantRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
