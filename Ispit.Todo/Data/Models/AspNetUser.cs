using Microsoft.AspNetCore.Identity;

namespace Ispit.Todo.Data.Models
{
    public class AspNetUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}