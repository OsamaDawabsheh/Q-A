using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI_Core.Helper;
using ProjectAPI_Core.DTOs._َQuestion;
using ProjectAPI_Core.Interfaces;
using ProjectAPI_Core.Models;
using ProjectAPI_Infrastructure.Repositories;
using ProjectAPI.Helper;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository questionRepository;

        public QuestionController(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionDto addQuestionDto)
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
                var result = await questionRepository.AddQuestion(addQuestionDto, addQuestionDto.UserId);

                if (result == "Question added successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            

        }

        [HttpPost("QuestionUsefulness")]
        public async Task<IActionResult> QuestionUsefulness([FromBody] QuestionUsefulnessDto questionUsefulnessDto)
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
                    return Unauthorized("Invalid user token.");
                }

                // Pass the userId to AddItemToCartAsync
                var result = await questionRepository.IsQuestionUseful(questionUsefulnessDto, questionUsefulnessDto.QuestionId, questionUsefulnessDto.UserId);

                if (result == "You canceled your vote on the question" || result == $"the question is {(questionUsefulnessDto.IsUseful ? "useful" : "useless")} for you ")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }


        [HttpGet("GetQuestions")]

        public async Task<IActionResult> GetQuestions([FromQuery] int page = 1, int pageSize = 20)
        {
            var result = await questionRepository.GetQuestions(page,pageSize);
            if (result == null)
            {
                return NotFound("questions not exists");
            }
            return Ok(result);
        }

        [HttpPut("EditQuestion/{id}")]
        public async Task<IActionResult> EditQuestion(EditQuestionDto editQuestionDto)
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
                var result = await questionRepository.EditQuestion(editQuestionDto, editQuestionDto.Id, editQuestionDto.UserId);

                if (result == "question updated successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }


        }

        [HttpDelete("DeleteQuestion/{id}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] int id, [FromBody] int usrId)
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
                var result = await questionRepository.DeleteQuestion(id, usrId);

                if (result == "question deleted successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }

        [HttpGet("GetQuestionDetails/{id}")]
        public async Task<IActionResult> GetQuestionDetails([FromRoute] int id)
        {
            var result = await questionRepository.GetQuestionDetails(id);

            if (result == null)
            {
                return NotFound("question not exist");
            }

            return Ok(result);

        }


        [HttpGet("GetUserQuestions/{usrId}")]

        public async Task<IActionResult> GetUserQuestions([FromRoute] int usrId , [FromQuery] int page = 1, int pageSize = 20)
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

                var result = await questionRepository.GetUserQuestions(usrId,page,pageSize);
                if (result == null && !result.Any())
                {
                    return NotFound("questions not exists");
                }
                return Ok(result);
    
        }


    }

}
