using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace dQRadca.Authorization
{
    public static class ProductOperations
    {
        public static OperationAuthorizationRequirement Create = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.CreateOperationName };
        public static OperationAuthorizationRequirement Read = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.ReadOperationName };
        public static OperationAuthorizationRequirement Update = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.UpdateOperationName };
        public static OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.DeleteOperationName };
    }
}