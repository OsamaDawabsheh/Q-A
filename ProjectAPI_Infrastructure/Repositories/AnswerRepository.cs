using Microsoft.EntityFrameworkCore;
using ProjectAPI_Core.DTOs._َQuestion;
using ProjectAPI_Core.DTOs.Answer;
using ProjectAPI_Core.DTOs.Comment;
using ProjectAPI_Core.DTOs.Tag;
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
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AppDbContext context;

        public AnswerRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<string> AddAnswer(AddAnswerDto addAnswerDto, int questionId, int userId)
        {
            var similarContentExists = await context.Answers.Where(a=>a.QuestionId == questionId)
             .AnyAsync(q => EF.Functions.Like(q.Content, $"%{addAnswerDto.Content}%"));

            if (similarContentExists)
            {
                return "An answer with a similar content already exists.";
            }

            var answer = new Answer
            {
                Content = addAnswerDto.Content,
                IsBest = false,
                QuestionId = questionId,
                UserId = userId
            };

            await context.Answers.AddAsync(answer);
            await context.SaveChangesAsync();

            return "answer added successfully";

        }

        public async Task<string> IsAnswerUseful(AnswerUsefulnessDto answerUsefulnessDto, int answerId, int userId)
        {
                var answer = await context.Answers.FirstOrDefaultAsync(q => q.Id == answerId);

                if (answer == null)
                {
                    return "answer not exist";
                }

                var existVote = await context.AnswerUsefulnesses.FirstOrDefaultAsync(au => au.UserId == userId && au.AnswerId == answerId);

                if (existVote != null)
                {
                    if (existVote.IsUseful == answerUsefulnessDto.IsUseful)
                    {
                        context.AnswerUsefulnesses.Remove(existVote);
                        await context.SaveChangesAsync();

                        return "You canceled your vote on the answer";

                    }
                    else
                    {
                        existVote.IsUseful = answerUsefulnessDto.IsUseful;
                        await context.SaveChangesAsync();
                        return $"the answer is {(answerUsefulnessDto.IsUseful ? "useful" : "useless")} for you ";

                    }
                }

                var answerUsefulness = new AnswerUsefulness
                {
                    AnswerId = answerId,
                    UserId = userId,
                    IsUseful = answerUsefulnessDto.IsUseful
                };

                await context.AnswerUsefulnesses.AddAsync(answerUsefulness);
                await context.SaveChangesAsync();

                return $"the answer is {(answerUsefulnessDto.IsUseful ? "useful" : "useless")} for you ";
            }

        public async Task<string> BestAnswer(BestAnswerDto bestAnswerDto, int questionId, int answerId, int userId)
        {
            var question = await context.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
            var answer = await context.Answers.FirstOrDefaultAsync(q => q.Id == answerId);

            if (question == null)
            {
                return "question not exist";
            }
            if (answer == null)
            {
                return "answer not exist";
            }

            if (question.UserId != userId) {
                return "you can't choose the best answer because you're not the writer of the question.";
            }

            var existBestAnswer = await context.Answers.FirstOrDefaultAsync(a => a.QuestionId == questionId && a.IsBest == true);

            if (existBestAnswer != null)
            {
                if (existBestAnswer.Id == answerId)
                {
                    existBestAnswer.IsBest = false;
                    await context.SaveChangesAsync();
                    return "You canceled the best answer";
                }
                else
                {
                    existBestAnswer.IsBest = false;
                    answer.IsBest = true;
                    await context.SaveChangesAsync();
                    return "You change the best answer";
                }

            }
                answer.IsBest = true;
                await context.SaveChangesAsync();
                return "You chose the best answer";
        }

        public async Task<string> EditAnswer(EditAnswerDto editAnswerDto, int answerId, int userId)
        {
            var answer = await context.Answers.FirstOrDefaultAsync(a => a.Id == answerId);

            if (answer == null)
            {
                return "answer not exist";
            }

            if (answer.UserId != userId)
            {
                return "you can't edit this answer because you are not the writer of answer";
            }

            answer.Content = editAnswerDto.Content;
            answer.UpdatedAt = DateTime.Now;

        

            await context.SaveChangesAsync();

            return "answer updated successfully";
        }

        public async Task<string> DeleteAnswer(int answerId, int userId)
        {
            var answer = await context.Answers.
            Include(a => a.answerUsefulnesses).
            Include(a => a.comments)
             .FirstOrDefaultAsync(a => a.Id == answerId);


            if (answer == null)
            {
                return "answer not exist";
            }

            if (answer.UserId != userId)
            {
                return "you can't delete this answer because you are not the writer of answer";
            }

            if (answer.answerUsefulnesses != null && answer.answerUsefulnesses.Any())
            {
                context.AnswerUsefulnesses.RemoveRange(answer.answerUsefulnesses);
                await context.SaveChangesAsync();

            }

            var comments = context.Comments.Where(a => a.Id == answerId).ToList();
            context.Comments.RemoveRange(comments);
            await context.SaveChangesAsync();


            context.Answers.Remove(answer);

            await context.SaveChangesAsync();

            return "answer deleted successfully";
        }

        public async Task<IEnumerable<GetAnswersDto>> GetUserAnswers(int userId,int page, int pageSize)
        {
            var answers = context.Answers.
                          Where(a => a.UserId == userId).
                          Include(a => a.answerUsefulnesses).
                          Include(a => a.comments).
                          ThenInclude(c => c.replies).
                          Select(a =>  new GetAnswersDto
                          {
                              Id = a.Id,
                              Content = a.Content,
                              CreatedAt = a.CreatedAt,
                              UpdatedAt = a.UpdatedAt,
                              IsBest = a.IsBest,
                              QuestionId = a.QuestionId,
                              UserId = a.UserId,
                              UsefulCount = a.answerUsefulnesses.Count(au => au.AnswerId == a.Id && au.IsUseful),
                              UselessCount = a.answerUsefulnesses.Count(au => au.AnswerId == a.Id && !au.IsUseful),
                              comments = a.comments.Select(c => new GetCommentsDto
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
                              }).ToList()
                          }).Skip((page - 1) * pageSize).Take(pageSize).Take(pageSize).AsQueryable();

            return (IEnumerable<GetAnswersDto>)answers;
        }
    }
}
