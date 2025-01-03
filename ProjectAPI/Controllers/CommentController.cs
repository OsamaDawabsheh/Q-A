using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI_Core.Helper;
using ProjectAPI_Core.DTOs._َQuestion;
using ProjectAPI_Core.DTOs.Comment;
using ProjectAPI_Core.DTOs.Tag;
using ProjectAPI_Core.Interfaces;
using ProjectAPI_Core.Models;
using ProjectAPI_Infrastructure.Repositories;
using ProjectAPI.Helper;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto addCommentDto)
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
                var result = await commentRepository.AddComment(addCommentDto, addCommentDto.QuestionId, addCommentDto.AnswerId, addCommentDto.UserId);
                if (result == "comment added successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }

        [HttpPost("AddReply")]
        public async Task<IActionResult> AddReply([FromBody] AddCommentDto addCommentDto)
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
                var result = await commentRepository.AddReply(addCommentDto, addCommentDto.QuestionId, addCommentDto.AnswerId, addCommentDto.UserId,addCommentDto.ParentCommentId);
                if (result == "reply added successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }


        [HttpPut("EditComment/{id}")]
        public async Task<IActionResult> EditComment([FromBody] EditCommentDto editCommentDto)
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
                var result = await commentRepository.EditComment(editCommentDto, editCommentDto.Id,editCommentDto.UserId);

                if (result == "comment updated successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }

        [HttpDelete("DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id, [FromBody] int usrId)
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
                var result = await commentRepository.DeleteComment(id , usrId);

                if (result == "comment deleted successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }

        [HttpGet("GetUserComments/{usrId}")]

        public async Task<IActionResult> GetUserComments([FromRoute] int usrId, [FromQuery] int page = 1, int pageSize = 20)
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

                var result = await commentRepository.GetUserComments(usrId,page,pageSize);
                if (result == null && !result.Any())
                {
                    return NotFound("comments not exists");
                }
                return Ok(result);


        }

    }
}
