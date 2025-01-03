using ProjectAPI_Core.DTOs.Answer;
using ProjectAPI_Core.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Interfaces
{
    public interface ICommentRepository
    {
        Task<string> AddComment(AddCommentDto addCommentDto , int questionId , int answerId , int userId);
        Task<string> AddReply(AddCommentDto addCommentDto, int questionId, int answerId, int userId , int? parentCommentId);

        Task<string> EditComment(EditCommentDto editCommentDto ,int commentId, int userId);

        Task<string> DeleteComment(int commentId ,int userId);
        Task<IEnumerable<GetCommentsDto>> GetUserComments(int userId , int page , int pageSize);




    }
}
