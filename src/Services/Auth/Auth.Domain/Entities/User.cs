namespace Auth.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public List<string> Roles { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public List<RefreshToken> RefreshTokens { get; private set; }

        private User()
        {
            Roles = new List<string>();
            RefreshTokens = new List<RefreshToken>();
        }

        public User(string email, string passwordHash, string firstName, string lastName, List<string>? roles = null)
        {
            Id = Guid.NewGuid();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Roles = roles ?? new List<string> { "User" };
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            RefreshTokens = new List<RefreshToken>();
        }

        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddRole(string role)
        {
            if (!Roles.Contains(role))
            {
                Roles.Add(role);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        public void RemoveRole(string role)
        {
            if (Roles.Contains(role))
            {
                Roles.Remove(role);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddRefreshToken(RefreshToken token)
        {
            RefreshTokens.Add(token);

            RemoveOldRefreshTokens();
        }

        private void RemoveOldRefreshTokens()
        {
            RefreshTokens.RemoveAll(x => !x.IsActive && x.CreatedAt.AddDays(2) <= DateTime.UtcNow);
        }

        public void RevokeRefreshToken(string token)
        {
            var refreshToken = RefreshTokens.FirstOrDefault(x => x.Token == token);
            if (refreshToken != null && refreshToken.IsActive)
            {
                refreshToken.Revoke();
            }
        }
    }
}
