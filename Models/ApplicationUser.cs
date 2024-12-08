using Microsoft.AspNetCore.Identity;

namespace librex3.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
