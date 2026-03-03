namespace Auth.Application.DTOs
{
    public record AuthResponse(
       string AccessToken,
       string RefreshToken,
       DateTime ExpiresAt,
       UserDto User
   );

    public record UserDto(
        Guid Id,
        string Email,
        string FirstName,
        string LastName,
        List<string> Roles
    );

}
