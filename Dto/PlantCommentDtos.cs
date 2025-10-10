namespace Plantpedia.DTO
{
    public class PlantCommentCreateRequest
    {
        public string PlantId { get; set; } = default!;
        public int UserId { get; set; }
        public string Content { get; set; } = default!;
        public int? ParentCommentId { get; set; }
    }

    public class PlantCommentReactionRequest
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public bool ReactionType { get; set; }
    }

    public class PlantCommentDto
    {
        public int CommentId { get; set; }
        public string PlantId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ParentCommentId { get; set; }
        public int ReactCount { get; set; }
        public List<PlantCommentDto> Replies { get; set; } = new();
    }

    public class PlantCommentUpdateRequest
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = default!;
    }

    public class PlantCommentDeleteRequest
    {
        public int CommentId { get; set; }
    }
}
