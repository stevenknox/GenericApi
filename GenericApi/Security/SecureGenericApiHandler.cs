using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace GenericApi
{
    public class SecureGenericApiHandler : AuthorizationHandler<SecureGenericApiRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SecureGenericApiRequirement requirement)
        {
            if (requirement.IsSecured)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
