using ProjectAPI_Core.DTOs._َQuestion;
using ProjectAPI_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Interfaces
{
    public interface IQuestionRepository
    {
        Task<string> AddQuestion(AddQuestionDto addQuestionDto , int UserId);
        Task<string> IsQuestionUseful(QuestionUsefulnessDto questionUsefulness,int questionId , int userId);

        Task<IEnumerable<GetQuestionsDto>> GetQuestions(int page, int pageSize);
        Task<string> EditQuestion(EditQuestionDto editQuestionDto ,int questionId, int userId);

        Task<string> DeleteQuestion(int questionId,int userId);

        Task<GetQuestionDetailsDto> GetQuestionDetails(int questionId);

        Task<IEnumerable<GetQuestionsDto>> GetUserQuestions(int userId , int page, int pageSize );


    }
}
