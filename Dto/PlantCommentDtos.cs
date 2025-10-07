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

        // 'L' = Like, 'D' = Dislike, 'N' = remove reaction
        public char ReactionType { get; set; }
    }

    public class PlantCommentDto
    {
        public int CommentId { get; set; }
        public string PlantId { get; set; } = default!;
        public int UserId { get; set; }
        public string UserName { get; set; } = "Ẩn danh";
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public int? ParentCommentId { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public List<PlantCommentDto> Replies { get; set; } = new();
    }
}
