using ProjectAPI_Core.DTOs._َQuestion;
using ProjectAPI_Core.DTOs.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Interfaces
{
    public interface IAnswerRepository
    {
        Task<string> AddAnswer(AddAnswerDto addAnswerDto, int questionId, int userId);
        Task<string> IsAnswerUseful(AnswerUsefulnessDto answerUsefulnessDto, int answerId, int userId);

        Task<string> BestAnswer(BestAnswerDto bestAnswerDto, int questionId,int answerId, int userId);
        Task<string> EditAnswer(EditAnswerDto editAnswerDto, int answerId, int userId);

        Task<string> DeleteAnswer(int answerId, int userId);

        Task<IEnumerable<GetAnswersDto>> GetUserAnswers(int userId , int page,int pageSize);


    }
}
