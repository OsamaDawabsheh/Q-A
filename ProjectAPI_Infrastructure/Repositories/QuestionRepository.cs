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
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext context;

        public QuestionRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<string> AddQuestion(AddQuestionDto addQuestionDto,int userId)
        {
            var similarTitleExists = await context.Questions
             .AnyAsync(q => EF.Functions.Like(q.Title, $"%{addQuestionDto.Title}%"));

            var similarContentExists = await context.Questions
             .AnyAsync(q => EF.Functions.Like(q.Content, $"%{addQuestionDto.Content}%"));

            if (similarTitleExists)
            {
                return "A question with a similar title already exists.";
            }

            if (similarContentExists)
            {
                return "A question with a similar Content already exists.";
            }

            var question = new Question
            {
                Title = addQuestionDto.Title,
                Content = addQuestionDto.Content,
                UserId = userId,
            };

            await context.Questions.AddAsync(question);
            await context.SaveChangesAsync();

            // Handle tags
            var QTags = new List<QuestionTag>();
            foreach (var tagName in addQuestionDto.Tags)
            {
                // Check if the tag already exists
                var tag = await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    // If the tag doesn't exist, create it
                    tag = new Tag
                    {
                        Name = tagName
                    };
                    await context.Tags.AddAsync(tag);
                    await context.SaveChangesAsync();
                }

                // Associate the tag with the question
                QTags.Add(new QuestionTag
                {
                    QuestionId = question.Id,
                    TagId = tag.Id
                });
            }

            question.questionTags = QTags;

            // Save to the database
            context.Questions.Update(question);
            await context.SaveChangesAsync();

            return "Question added successfully";
        }


        public async Task<string> IsQuestionUseful(QuestionUsefulnessDto questionUsefulnessDto, int questionId, int userId)
        {
            var question = await context.Questions.FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
            {
                return "question not exist";
            }

            var existVote = await context.QuestionUsefulnesses.FirstOrDefaultAsync(qu=>qu.UserId == userId && qu.QuestionId == questionId);
    
            if (existVote != null)
            {
                if (existVote.IsUseful == questionUsefulnessDto.IsUseful)
                {
                    context.QuestionUsefulnesses.Remove(existVote);
                    await context.SaveChangesAsync();

                    return "You canceled your vote on the question";

                }
                else {
                    existVote.IsUseful = questionUsefulnessDto.IsUseful;
                    await context.SaveChangesAsync();
                    return $"the question is {(questionUsefulnessDto.IsUseful ? "useful" : "useless")} for you ";

                }
            }

            var questionUsefulness = new QuestionUsefulness
            {
                QuestionId = questionId,
                UserId = userId,
                IsUseful = questionUsefulnessDto.IsUseful
            };

            await context.QuestionUsefulnesses.AddAsync(questionUsefulness);
            await context.SaveChangesAsync();

            return $"the question is {(questionUsefulnessDto.IsUseful ? "useful" : "useless")} for you ";
        }


        public async Task<IEnumerable<GetQuestionsDto>> GetQuestions(int page, int pageSize)
        {
            var questions = context.Questions.
                            Include(q => q.questionTags).
                            ThenInclude(qt => qt.tag).
                            Include(q => q.questionUsefulnesses).
                            Include(q => q.answers).
                             Select(q => new GetQuestionsDto
                             {
                                 Id = q.Id,
                                 Title = q.Title,
                                 Content = q.Content,
                                 CreatedAt = q.CreatedAt,
                                 UpdatedAt = q.UpdatedAt,
                                 UserId = q.UserId,
                                 UsefulCount = q.questionUsefulnesses.Count(qu => qu.IsUseful),
                                 UselessCount = q.questionUsefulnesses.Count(qu => !qu.IsUseful),
                                 AnswersCount = q.answers.Count(),
                                 Tags = q.questionTags.Select(qt => new GetTagsDto
                                 {
                                     Id = qt.tag.Id,
                                     Name = qt.tag.Name,
                                     CreatedAt = qt.tag.CreatedAt
                                 }).ToList()

                             }).Skip((page - 1) * pageSize).Take(pageSize).Take(pageSize).AsQueryable();

            return (IEnumerable<GetQuestionsDto>)questions;
        }

        public async Task<string> EditQuestion(EditQuestionDto editQuestionDto, int questionId, int userId)
        {

            var question = await context.Questions.FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
            {
                return "question not exist";
            }

            if(question.UserId != userId)
            {
                return "you can't edit this question because you are not the writer of question";
            }

            var similarTitleExists = await context.Questions
             .AnyAsync(q => EF.Functions.Like(q.Title, $"%{editQuestionDto.Title}%"));

            var similarContentExists = await context.Questions
             .AnyAsync(q => EF.Functions.Like(q.Content, $"%{editQuestionDto.Content}%"));

            if (similarTitleExists)
            {
                return "A question with a similar title already exists.";
            }

            if (similarContentExists)
            {
                return "A question with a similar Content already exists.";
            }

            question.Title = editQuestionDto.Title;
            question.Content = editQuestionDto.Content;
            question.UpdatedAt = DateTime.UtcNow;

            // Handle tags
            var oldQTags = await context.QuestionTags.Where(qt=>qt.QuestionId == questionId).ToListAsync();
            oldQTags.RemoveRange(0, oldQTags.Count);

            var newQTags = new List<QuestionTag>();
            foreach (var tagName in editQuestionDto.Tags)
            {
                // Check if the tag already exists
                var tag = await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    // If the tag doesn't exist, create it
                    tag = new Tag
                    {
                        Name = tagName
                    };
                    await context.Tags.AddAsync(tag);
                    await context.SaveChangesAsync();
                }
                tag.UpdatedAt = DateTime.UtcNow;

                // Associate the tag with the question
                newQTags.Add(new QuestionTag
                {
                    QuestionId = question.Id,
                    TagId = tag.Id
                });
            }

            question.questionTags = newQTags;

            await context.SaveChangesAsync();

            return "question updated successfully";
        }

        public async Task<string> DeleteQuestion(int questionId, int userId)
        {
            var question = await context.Questions.
                Include(q=>q.questionTags).
                Include(q=>q.answers).ThenInclude(a=>a.answerUsefulnesses).
                Include(q => q.answers).ThenInclude(a => a.comments).
                Include(q=>q.questionUsefulnesses).
                FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
            {
                return "question not exist";
            }

            if (question.UserId != userId)
            {
                return "you can't delete this question because you are not the writer of question";
            }


            context.QuestionTags.RemoveRange(question.questionTags);
            context.QuestionUsefulnesses.RemoveRange(question.questionUsefulnesses);
            foreach (var answer in question.answers)
            {
                context.AnswerUsefulnesses.RemoveRange(answer.answerUsefulnesses);
                context.Comments.RemoveRange(answer.comments);
            }
            await context.SaveChangesAsync();

            context.Answers.RemoveRange(question.answers);
            await context.SaveChangesAsync();

            context.Questions.Remove(question);

            await context.SaveChangesAsync();

            return "question deleted successfully";
        }

        public async Task<GetQuestionDetailsDto> GetQuestionDetails(int questionId)
        {
            var question = await context.Questions
                .Include(q=>q.user).
                Include(q=>q.questionTags).ThenInclude(qt=>qt.tag).
                Include(q=>q.answers).ThenInclude(a => a.answerUsefulnesses).
                Include(q => q.answers).ThenInclude(a => a.comments).ThenInclude(c=>c.replies).
                Include(q=>q.questionUsefulnesses)
                .FirstOrDefaultAsync(q=>q.Id == questionId);

            if (question == null)
            {
                throw new KeyNotFoundException($"Question with ID {questionId} not found.");
            }

            var getQuestionDetailsDto = new GetQuestionDetailsDto
            {
                Title = question.Title,
                Content = question.Content,
                UserId = question.UserId,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt,
                UsefulCount = question.questionUsefulnesses.Count( qu => qu.QuestionId == questionId && qu.IsUseful),
                UselessCount = question.questionUsefulnesses.Count(qu => qu.QuestionId == questionId && !qu.IsUseful),
                Tags = question.questionTags.Select(qt => new GetTagsDto
                {
                    Id = qt.tag.Id,
                    Name = qt.tag.Name
                }).ToList(),
                Answers = question.answers.Select(a => new GetAnswersDto
                {
                    Id = a.Id,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt= a.UpdatedAt,
                    IsBest = a.IsBest,
                    QuestionId = a.QuestionId,
                    UserId = a.UserId,
                    UsefulCount = a.answerUsefulnesses.Count(au=> au.AnswerId == a.Id && au.IsUseful),
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
                }).ToList()
            };


            return getQuestionDetailsDto;

        }

        public async Task<IEnumerable<GetQuestionsDto>> GetUserQuestions(int userId, int page, int pageSize)
        {
            var questions = context.Questions.
                            Where(q => q.UserId == userId).
                            Include(q => q.questionTags).
                            ThenInclude(qt => qt.tag).
                            Include(q => q.questionUsefulnesses).
                            Include(q => q.answers).
                            Select(q => new GetQuestionsDto
                             {
                                 Id = q.Id,
                                 Title = q.Title,
                                 Content = q.Content,
                                 CreatedAt = q.CreatedAt,
                                 UpdatedAt = q.UpdatedAt,
                                 UserId = q.UserId,
                                 UsefulCount = q.questionUsefulnesses.Count(qu => qu.IsUseful),
                                 UselessCount = q.questionUsefulnesses.Count(qu => !qu.IsUseful),
                                 AnswersCount = q.answers.Count(),
                                 Tags = q.questionTags.Select(qt => new GetTagsDto
                                 {
                                     Id = qt.tag.Id,
                                     Name = qt.tag.Name,
                                     CreatedAt = qt.tag.CreatedAt,
                                     UpdatedAt = qt.tag.UpdatedAt,
                                 }).ToList()

                             }).Skip((page - 1) * pageSize).Take(pageSize).Take(pageSize).AsQueryable();

            return (IEnumerable<GetQuestionsDto>)questions;
        }
    }
}
