using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Repository;

namespace Plantpedia.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;
        private readonly IUserRepository _userRepo;

        // private readonly AzureContentSafetyService _azureContentSafetyService;

        public CommentService(
            ICommentRepository repo,
            IUserRepository userRepo
        // AzureContentSafetyService azureContentSafetyService
        )
        {
            _repo = repo;
            _userRepo = userRepo;
            // _azureContentSafetyService = azureContentSafetyService;
        }

        public async Task<List<PlantCommentDto>> GetCommentsByPlantAsync(
            string plantId,
            int? userId
        )
        {
            var list = await _repo.GetByPlantAsync(plantId);

            PlantCommentDto Map(PlantComment c) =>
                new PlantCommentDto
                {
                    CommentId = c.CommentId,
                    PlantId = c.PlantId,
                    UserId = c.UserId,
                    UserName = c.User?.LastName ?? "Người dùng ẩn danh",
                    AvatarUrl = c.User?.AvatarUrl ?? "/img/default-avatar.png",
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ParentCommentId = c.ParentCommentId,
                    ReactCount = c.Reactions?.Count(r => r.ReactionType == true) ?? 0,
                    IsReactedByCurrentUser =
                        userId.HasValue
                        && (
                            c.Reactions?.Any(r =>
                                r.UserId == userId.Value && r.ReactionType == true
                            ) ?? false
                        ),
                    Replies = c.Replies?.OrderBy(x => x.CreatedAt).Select(Map).ToList() ?? new(),
                };

            return list.Where(c => c.ParentCommentId == null)
                .OrderByDescending(r => r.CreatedAt)
                .Select(Map)
                .ToList();
        }

        public async Task<PlantCommentDto> CreateAsync(
            PlantCommentCreateRequest request,
            int userId
        )
        {
            if (string.IsNullOrWhiteSpace(request.PlantId))
                throw new ArgumentException("Thiếu PlantId");
            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentException("Nội dung không được để trống");

            // var analysisResult = await _azureContentSafetyService.AnalyzeContentAsync(
            //     request.Content
            // );
            // if (analysisResult.IsHarmful)
            // {
            //     throw new ArgumentException(analysisResult.Reason);
            // }

            var entity = new PlantComment
            {
                PlantId = request.PlantId,
                UserId = userId,
                Content = request.Content.Trim(),
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            var user = await _userRepo.GetUserByIdAsync(userId);

            return new PlantCommentDto
            {
                CommentId = entity.CommentId,
                PlantId = entity.PlantId,
                UserId = entity.UserId,
                UserName = user?.LastName ?? "Người dùng ẩn danh",
                AvatarUrl = user?.AvatarUrl ?? "/img/default-avatar.png",
                Content = entity.Content,
                CreatedAt = entity.CreatedAt,
                ParentCommentId = entity.ParentCommentId,
                ReactCount = 0,
                Replies = new(),
            };
        }

        public async Task<int> ToggleReactionAsync(PlantCommentReactionRequest request, int userId)
        {
            if (request.CommentId <= 0)
                throw new ArgumentException("Thiếu thông tin bình luận");

            var comment = await _repo.GetByIdAsync(request.CommentId);
            if (comment == null)
                throw new ArgumentException("Bình luận không tồn tại");

            var existingReact = await _repo.GetReactionAsync(request.CommentId, userId);

            if (request.ReactionType)
            {
                // LIKE: Nếu chưa like thì thêm, đã like thì bỏ qua (idempotent)
                if (existingReact == null)
                    await _repo.UpsertReactionAsync(request.CommentId, userId, true);
                // Nếu đã like, không làm gì (idempotent)
            }
            else
            {
                // UNLIKE: Nếu đã like thì xoá, chưa like thì bỏ qua
                if (existingReact != null)
                    await _repo.DeleteReactionAsync(request.CommentId, userId);
                // Nếu chưa like, không làm gì (idempotent)
            }

            await _repo.SaveChangesAsync();

            int reactCount = await _repo.CountReactionsAsync(request.CommentId);
            return reactCount;
        }

        public async Task UpdateAsync(PlantCommentUpdateRequest request, int userId)
        {
            LoggerHelper.Info($"User={userId} bắt đầu cập nhật CommentId={request.CommentId}.");

            var comment = await _repo.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                LoggerHelper.Warn(
                    $"Cập nhật thất bại: không tìm thấy CommentId={request.CommentId}."
                );
                throw new ArgumentException("Bình luận không tồn tại.");
            }

            if (comment.UserId != userId)
            {
                LoggerHelper.Warn(
                    $"Cố gắng cập nhật trái phép: User={userId} không phải chủ sở hữu của CommentId={request.CommentId}."
                );
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền chỉnh sửa bình luận này."
                );
            }
            // var analysisResult = await _azureContentSafetyService.AnalyzeContentAsync(
            //     request.Content
            // );
            // if (analysisResult.IsHarmful)
            // {
            //     throw new ArgumentException(analysisResult.Reason);
            // }
            comment.Content = request.Content.Trim();
            await _repo.SaveChangesAsync();

            LoggerHelper.Info(
                $"User={userId} đã cập nhật thành công CommentId={request.CommentId}."
            );
        }

        public async Task DeleteAsync(int commentId, int userId)
        {
            LoggerHelper.Info($"User={userId} bắt đầu xóa CommentId={commentId}.");

            var comment = await _repo.GetByIdAsync(commentId);
            if (comment == null)
            {
                LoggerHelper.Warn($"Xóa thất bại: không tìm thấy CommentId={commentId}.");
                throw new ArgumentException("Bình luận không tồn tại.");
            }

            if (comment.UserId != userId)
            {
                LoggerHelper.Warn(
                    $"Cố gắng xóa trái phép: User={userId} không phải chủ sở hữu của CommentId={commentId}."
                );
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bình luận này.");
            }

            _repo.Delete(comment);
            await _repo.SaveChangesAsync();

            LoggerHelper.Info(
                $"User={userId} đã xóa thành công (soft delete) CommentId={commentId}."
            );
        }
    }
}
