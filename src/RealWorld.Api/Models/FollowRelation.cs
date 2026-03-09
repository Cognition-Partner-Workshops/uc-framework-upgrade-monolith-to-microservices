namespace RealWorld.Api.Models;

public class FollowRelation
{
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    public string TargetId { get; set; } = string.Empty;
    public User? Target { get; set; }
}
