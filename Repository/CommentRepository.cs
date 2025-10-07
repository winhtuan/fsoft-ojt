using Microsoft.EntityFrameworkCore;
using PLANTINFOWEB.Data;
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
            // Log
            LoggerHelper.Info($"Tải comment theo plant {plantId}");

            // Lấy thread theo cây (bao gồm user, replies, reactions)
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
                .FirstOrDefaultAsync(c => c.CommentId == id);
        }

        public async Task AddAsync(PlantComment comment)
        {
            await _context.PlantComments.AddAsync(comment);
        }

        public async Task UpsertReactionAsync(int commentId, int userId, char reactionType)
        {
            var react = await _context.PlantCommentReactions.FirstOrDefaultAsync(x =>
                x.CommentId == commentId && x.UserId == userId
            );

            if (reactionType == 'N')
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
                        ReactionType = reactionType,
                        CreatedAt = DateTime.UtcNow,
                    }
                );
            }
            else
            {
                react.ReactionType = reactionType;
                react.CreatedAt = DateTime.UtcNow;
            }
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
