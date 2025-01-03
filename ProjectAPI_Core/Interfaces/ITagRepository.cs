using ProjectAPI_Core.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Interfaces
{
    public interface ITagRepository
    {
        Task<string> AddTag(AddTagDto addTagDto);
        Task<IEnumerable<GetTagsDto>> GetTags(int page , int pageSize);

        Task<string> EditTag(int tagId,string tagName);
        Task<string> DeleteTag(int tagId);

    }
}
