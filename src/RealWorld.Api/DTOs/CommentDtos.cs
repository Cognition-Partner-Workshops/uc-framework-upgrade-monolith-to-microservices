using System.ComponentModel.DataAnnotations;

namespace RealWorld.Api.DTOs;

public class NewCommentRequest
{
    public NewCommentDto Comment { get; set; } = new();
}

public class NewCommentDto
{
    [Required(ErrorMessage = "can't be empty")]
    public string Body { get; set; } = string.Empty;
}

public class SingleCommentResponse
{
    public CommentDataDto Comment { get; set; } = new();
}

public class MultipleCommentsResponse
{
    public List<CommentDataDto> Comments { get; set; } = new();
}

public class CommentDataDto
{
    public string Id { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ProfileDataDto Author { get; set; } = new();
}
