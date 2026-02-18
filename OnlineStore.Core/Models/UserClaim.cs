namespace OnlineStore.Core.Models;

public class UserClaim
{
    public Guid Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
}