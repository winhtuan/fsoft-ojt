using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Models;

namespace Plantpedia.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlantComment>> GetByPlantAsync(string plantId)
        {
            return await _context
                .PlantComments.Where(c => c.PlantId == plantId && !c.IsDeleted)
                .Include(c => c.User)
                .Include(c => c.Reactions)
                .Include(c => c.Replies.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.User)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Reactions)
                .AsSplitQuery()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<PlantComment?> GetByIdAsync(int id)
        {
            return await _context
                .PlantComments.Include(c => c.User)
                .Include(c => c.Reactions)
                .FirstOrDefaultAsync(c => c.CommentId == id && !c.IsDeleted);
        }

        public async Task AddAsync(PlantComment comment)
        {
            await _context.PlantComments.AddAsync(comment);
        }

        public async Task UpdateAsync(PlantCommentUpdateRequest request, int userId)
        {
            var comment = await GetByIdAsync(request.CommentId);
            if (comment == null)
                throw new ArgumentException("Bình luận không tồn tại.");
            if (comment.UserId != userId)
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền chỉnh sửa bình luận này."
                );

            comment.Content = request.Content.Trim();
        }

        public async Task DeleteAsync(int commentId, int userId)
        {
            var comment = await GetByIdAsync(commentId);
            if (comment == null)
                throw new ArgumentException("Bình luận không tồn tại.");
            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bình luận này.");

            Delete(comment);
        }

        public async Task DeleteReactionAsync(int commentId, int userId)
        {
            var entity = await _context.PlantCommentReactions.FirstOrDefaultAsync(r =>
                r.CommentId == commentId && r.UserId == userId
            );
            if (entity != null)
                _context.PlantCommentReactions.Remove(entity);
        }

        public async Task<PlantCommentReaction?> GetReactionAsync(int commentId, int userId)
        {
            return await _context.PlantCommentReactions.FirstOrDefaultAsync(x =>
                x.CommentId == commentId && x.UserId == userId
            );
        }

        public async Task<int> CountReactionsAsync(int commentId)
        {
            return await _context.PlantCommentReactions.CountAsync(r =>
                r.CommentId == commentId && r.ReactionType == true
            );
        }

        public async Task UpsertReactionAsync(int commentId, int userId, bool isReacting)
        {
            var react = await GetReactionAsync(commentId, userId);

            if (!isReacting)
            {
                if (react != null)
                    _context.PlantCommentReactions.Remove(react);
                return;
            }

            if (react == null)
            {
                await _context.PlantCommentReactions.AddAsync(
                    new PlantCommentReaction
                    {
                        CommentId = commentId,
                        UserId = userId,
                        ReactionType = true,
                        CreatedAt = DateTime.UtcNow,
                    }
                );
            }
            else
            {
                react.CreatedAt = DateTime.UtcNow;
            }
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Delete(PlantComment comment)
        {
            comment.IsDeleted = true;
            _context.PlantComments.Update(comment);
        }
    }
}
