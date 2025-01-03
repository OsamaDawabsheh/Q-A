using Microsoft.EntityFrameworkCore;
using ProjectAPI_Core.DTOs.Answer;
using ProjectAPI_Core.DTOs.Comment;
using ProjectAPI_Core.Interfaces;
using ProjectAPI_Core.Models;
using ProjectAPI_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext context;

        public CommentRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<string> AddComment(AddCommentDto addCommentDto, int questionId, int answerId, int userId)
        {
            var similarContentExists = await context.Comments
                .AnyAsync(q => EF.Functions.Like(q.Content, $"%{addCommentDto.Content}%"));

            if (similarContentExists)
            {
                return "An comment with a similar content already exists.";
            }

            var comment = new Comment
            {
                Content = addCommentDto.Content,
                QuestionId = questionId,
                UserId = userId,
                AnswerId = answerId,
                ParentCommentId = null
            };

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            return "comment added successfully";
        }

        public async Task<string> AddReply(AddCommentDto addCommentDto, int questionId, int answerId, int userId, int? parentCommentId)
        {
            var parentComment = await context.Comments.FirstOrDefaultAsync(c => c.Id == parentCommentId);

            if (parentComment == null)
            {
                return "The comment you are trying to reply to is not exist";
            }

            var similarContentExists = await context.Comments
                    .AnyAsync(q => EF.Functions.Like(q.Content, $"%{addCommentDto.Content}%"));

            if (similarContentExists)
            {
                return "A reply with a similar content already exists.";
            }

            var reply = new Comment
            {
                Content = addCommentDto.Content,
                QuestionId = questionId,
                UserId = userId,
                AnswerId = answerId,
                ParentCommentId = parentCommentId.Value
            };

            await context.Comments.AddAsync(reply);
            await context.SaveChangesAsync();

            return "reply added successfully";
        }

        public async Task<string> EditComment(EditCommentDto editCommentDto,int commentId, int userId)
        {
            
                var comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                {
                    return "comment not exist";
                }

            if (comment.UserId != userId)
            {
                return "you can't edit this comment because you are not the writer of comment";

            }


            comment.Content = editCommentDto.Content;
            comment.UpdatedAt = DateTime.Now;

                await context.SaveChangesAsync();

                return "comment updated successfully";
        }

        public async Task<string> DeleteComment(int commentId ,int userId)
        {
            var comment = await context.Comments.Include(c => c.replies).FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return "comment not exist";
            }

            if (comment.UserId != userId)
            {
                return "you can't delete this comment because you are not the writer of comment";

            }

            if (comment.replies != null && comment.replies.Any())
            {
            context.Comments.RemoveRange(comment.replies);
                await context.SaveChangesAsync();

            }

            context.Comments.Remove(comment);
            await context.SaveChangesAsync();

            return "comment deleted successfully";
        }

        public async Task<IEnumerable<GetCommentsDto>> GetUserComments(int userId,int page,int pageSize)
        {
            var comments = context.Comments.
                     Where(c => c.UserId == userId).
                     Include(c => c.replies).
                     Select(c => new GetCommentsDto
                     {
                             Id = c.Id,
                             Content = c.Content,
                             CreatedAt = c.CreatedAt,
                             UpdatedAt = c.UpdatedAt,
                             QuestionId = c.QuestionId,
                             UserId = c.UserId,
                             AnswerId = c.AnswerId,
                             ParentCommentId = c.ParentCommentId,
                             Replies = c.replies.Select(r => new GetRepliesDto
                             {
                                 Id = r.Id,
                                 Content = r.Content,
                                 CreatedAt = r.CreatedAt,
                                 UpdatedAt = r.UpdatedAt,
                                 QuestionId = r.QuestionId,
                                 UserId = r.UserId,
                                 AnswerID = r.AnswerId,
                                 ParentCommentId = r.ParentCommentId
                             }).ToList()
                     }).Skip((page - 1) * pageSize).Take(pageSize).Take(pageSize).AsQueryable();

            return (IEnumerable<GetCommentsDto>)comments;
        }
    }
}
