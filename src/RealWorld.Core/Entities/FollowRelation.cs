namespace RealWorld.Core.Entities;

public class FollowRelation
{
    public string UserId { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;

    public FollowRelation() { }

    public FollowRelation(string userId, string targetId)
    {
        UserId = userId;
        TargetId = targetId;
    }
}
