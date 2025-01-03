using Microsoft.EntityFrameworkCore;
using ProjectAPI_Core.DTOs._َQuestion;
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
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext context;

        public TagRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<string> AddTag(AddTagDto addTagDto)
        {
            var existTag = await context.Tags.FirstOrDefaultAsync(t=>t.Name == addTagDto.Name);

            if (existTag != null) {
                return "Tag already exist";
            }

            var tag = new Tag
            {
                Name = addTagDto.Name,
            };

            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();

            return "tag added successfully";
        }


        public async Task<IEnumerable<GetTagsDto>> GetTags(int page , int pageSize)
        {
            var tags =  context.Tags.Include(t=>t.questionTags).Select(t => new GetTagsDto
            {
                Id = t.Id,
                Name=t.Name,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                QuestionsCount = t.questionTags.Count(),

            }).Skip((page - 1) * pageSize).Take(pageSize).Take(pageSize).AsQueryable();

            return (IEnumerable<GetTagsDto>) tags;
        }


        public async Task<string> EditTag(int tagId, string tagName)
        {
            var tag = await context.Tags.FirstOrDefaultAsync(t=> t.Id == tagId);

            if (tag == null)
            {
                return "tag not exist";
            }

            var similarNameExists = await context.Tags
                 .AnyAsync(q => EF.Functions.Like(q.Name, $"%{tagName}%"));


            if (similarNameExists)
            {
                return "A tag with a similar name already exists.";
            }

            tag.Name = tagName;
            tag.UpdatedAt = DateTime.Now;

            await context.SaveChangesAsync();

            return "tag updated successfully";
        }

        public async Task<string> DeleteTag(int tagId)
        {
            var tag = await context.Tags.Include(t=>t.questionTags).FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
            {
                return "tag not exist";
            }
            context.QuestionTags.RemoveRange(tag.questionTags);
             context.Tags.Remove(tag);
            await context.SaveChangesAsync();

            return "tag deleted successfully";
        }


    }
}
