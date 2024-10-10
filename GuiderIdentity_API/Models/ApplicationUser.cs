using Microsoft.AspNetCore.Identity;

namespace GuiderIdentity_API.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string? Name { get; set; }
    }
}
