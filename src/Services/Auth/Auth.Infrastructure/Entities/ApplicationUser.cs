using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Entities;


public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ApplicationUser()
    {
        Id = Guid.NewGuid();
    }

    public ApplicationUser(string email, string userName) : base(userName)
    {
        Id = Guid.NewGuid();
        Email = email;
        UserName = userName;
    }
}
