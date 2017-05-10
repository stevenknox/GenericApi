using Microsoft.AspNetCore.Authorization;

namespace GenericApi
{
    public class SecureGenericApiRequirement : IAuthorizationRequirement
    {
        public SecureGenericApiRequirement(ApiAuthorization auth)
        {
            IsSecured = auth == ApiAuthorization.Authorize;
        }

        public bool IsSecured { get; set; }
    }

    public enum ApiAuthorization
    {
        Authorize,
        AllowAnonymous
    }
}
