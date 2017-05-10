using Microsoft.AspNetCore.Authorization;

namespace GenericApi
{
    public class SecureGenericApiRequirement : IAuthorizationRequirement
    {
        public SecureGenericApiRequirement(bool isSecured)
        {
            IsSecured = isSecured;
        }

        public bool IsSecured { get; set; }
    }
}
