using Microsoft.AspNetCore.Authorization;

namespace OnlineStore.Core.Authorization;

public class ResourceOwnerRequirement : IAuthorizationRequirement
{
    public string ResourceType { get; }

    public ResourceOwnerRequirement(string resourceType)
    {
        ResourceType = resourceType;
    }
}