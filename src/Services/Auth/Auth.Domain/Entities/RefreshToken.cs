namespace Auth.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string CreatedByIp { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedByIp { get; private set; }
        public string? ReplacedByToken { get; private set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        private RefreshToken() { }

        public RefreshToken(string token, DateTime expiresAt, string createdByIp)
        {
            Id = Guid.NewGuid();
            Token = token ?? throw new ArgumentNullException(nameof(token));
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            CreatedByIp = createdByIp ?? throw new ArgumentNullException(nameof(createdByIp));
        }

        public void Revoke(string? revokedByIp = null, string? replacedByToken = null)
        {
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = revokedByIp;
            ReplacedByToken = replacedByToken;
        }
    }
}
