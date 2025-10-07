using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Repository;

namespace Plantpedia.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;

        public CommentService(ICommentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PlantCommentDto>> GetCommentsByPlantAsync(string plantId)
        {
            LoggerHelper.Info($"Lấy danh sách comment cho cây: {plantId}");
            var list = await _repo.GetByPlantAsync(plantId);

            // Build tree (root = comment ParentCommentId == null)
            var dict = list.ToDictionary(c => c.CommentId);
            var roots = list.Where(c => c.ParentCommentId == null).ToList();

            PlantCommentDto Map(PlantComment c) =>
                new PlantCommentDto
                {
                    CommentId = c.CommentId,
                    PlantId = c.PlantId,
                    UserId = c.UserId,
                    UserName = c.User?.LastName ?? "Ẩn danh",
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ParentCommentId = c.ParentCommentId,
                    LikeCount = c.Reactions?.Count(r => r.ReactionType == 'L') ?? 0,
                    DislikeCount = c.Reactions?.Count(r => r.ReactionType == 'D') ?? 0,
                    Replies = c.Replies?.OrderBy(x => x.CreatedAt).Select(Map).ToList() ?? new(),
                };

            return roots.OrderByDescending(r => r.CreatedAt).Select(Map).ToList();
        }

        public async Task<PlantCommentDto> CreateAsync(PlantCommentCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PlantId))
                throw new ArgumentException("Thiếu PlantId");
            if (request.UserId <= 0)
                throw new ArgumentException("Thiếu UserId");
            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentException("Nội dung không được để trống");

            LoggerHelper.Info(
                $"Tạo comment mới Plant={request.PlantId}, User={request.UserId}, Parent={request.ParentCommentId}"
            );

            var entity = new PlantComment
            {
                PlantId = request.PlantId,
                UserId = request.UserId,
                Content = request.Content.Trim(),
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // map DTO đơn giản
            return new PlantCommentDto
            {
                CommentId = entity.CommentId,
                PlantId = entity.PlantId,
                UserId = entity.UserId,
                UserName = "Bạn",
                Content = entity.Content,
                CreatedAt = entity.CreatedAt,
                ParentCommentId = entity.ParentCommentId,
                LikeCount = 0,
                DislikeCount = 0,
                Replies = new(),
            };
        }

        public async Task ToggleReactionAsync(PlantCommentReactionRequest request)
        {
            if (request.UserId <= 0 || request.CommentId <= 0)
                throw new ArgumentException("Thiếu thông tin người dùng hoặc bình luận");

            LoggerHelper.Info(
                $"Reaction Comment={request.CommentId}, User={request.UserId}, Type={request.ReactionType}"
            );
            await _repo.UpsertReactionAsync(
                request.CommentId,
                request.UserId,
                request.ReactionType
            );
            await _repo.SaveChangesAsync();
        }
    }
}
