using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI_Core.Helper;
using ProjectAPI_Core.DTOs.Answer;
using ProjectAPI_Core.DTOs.Tag;
using ProjectAPI_Core.Interfaces;
using ProjectAPI_Infrastructure.Repositories;
using ProjectAPI.Helper;
using Microsoft.AspNetCore.Authorization;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository tagRepository;

        public TagController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpPost("AddTag")]
        public async Task<IActionResult> AddTag([FromBody] AddTagDto addTagDto)
        {
            // Extract the token from the Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing");
            }


                // Extract user ID from the token
                var userId = ExtractClaims.ExtractUserIdFromToken(token);

                if (!userId.HasValue)
                {
                    return Unauthorized("Invalid user  token.");
                }

                // Pass the userId to AddItemToCartAsync
                var result = await tagRepository.AddTag(addTagDto);

                if (result == "tag added successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            
        }

        [HttpGet("GetTags")]

        public async Task<IActionResult> GetTags([FromQuery] int page = 1, int pageSize = 20)
        {
            var tags = await tagRepository.GetTags(page,pageSize);
            if (tags == null)
            {
                return NotFound("tags not exists");
            }
            return Ok(tags);
        }

        [HttpPut("EditTag/{id}")]
        public async Task<IActionResult> EditTag([FromBody] EditTagDto editTagDto )
        {
            // Extract the token from the Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing");
            }


                // Extract user ID from the token
                var userId = ExtractClaims.ExtractUserIdFromToken(token);

                if (!userId.HasValue)
                {
                    return Unauthorized("Invalid user  token.");
                }

                // Pass the userId to AddItemToCartAsync
                var result = await tagRepository.EditTag(editTagDto.Id,editTagDto.Name);

                if (result == "tag updated successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }

        [HttpDelete("DeleteTag/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditTag([FromRoute] int id)
        {
            // Extract the token from the Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing");
            }

                // Extract user ID from the token
                var userId = ExtractClaims.ExtractUserIdFromToken(token);

                if (!userId.HasValue)
                {
                    return Unauthorized("Invalid user  token.");
                }

                // Pass the userId to AddItemToCartAsync
                var result = await tagRepository.DeleteTag(id);

                if (result == "tag deleted successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
 
        }

    }
}
