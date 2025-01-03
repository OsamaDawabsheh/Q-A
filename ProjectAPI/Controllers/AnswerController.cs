using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI_Core.Helper;
using ProjectAPI_Core.DTOs._َQuestion;
using ProjectAPI_Core.DTOs.Answer;
using ProjectAPI_Core.Interfaces;
using ProjectAPI_Infrastructure.Repositories;
using ProjectAPI.Helper;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerRepository answerRepository;

        public AnswerController(IAnswerRepository answerRepository)
        {
            this.answerRepository = answerRepository;
        }

        [HttpPost("AddAnswer")]
        public async Task<IActionResult> AddAnswer([FromBody] AddAnswerDto addAnswerDto)
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
                var result = await answerRepository.AddAnswer(addAnswerDto,addAnswerDto.QuestionId, addAnswerDto.UserId);

                if (result == "answer added successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }

        [HttpPost("AnswerUsefulness")]
        public async Task<IActionResult> AnswerUsefulness([FromBody] AnswerUsefulnessDto answerUsefulnessDto)
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
                var result = await answerRepository.IsAnswerUseful(answerUsefulnessDto, answerUsefulnessDto.AnswerId, answerUsefulnessDto.UserId);

                if (result == "You canceled your vote on the answer" || result == $"the answer is {(answerUsefulnessDto.IsUseful ? "useful" : "useless")} for you ") 
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }


        [HttpPost("BestAnswer")]
        public async Task<IActionResult> BestAnswer([FromBody] BestAnswerDto bestAnswerDto)
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
                var result = await answerRepository.BestAnswer(bestAnswerDto, bestAnswerDto.QuestionId, bestAnswerDto.Id, bestAnswerDto.UserId);

                if (result.EndsWith("best answer"))
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }


        [HttpPut("EditAnswer/{id}")]
        public async Task<IActionResult> EditAnswer(EditAnswerDto editAnswerDto)
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
                var result = await answerRepository.EditAnswer(editAnswerDto, editAnswerDto.Id, editAnswerDto.UserId);

                if (result == "answer updated successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }


        }


        [HttpDelete("DeleteAnswer/{id}")]
        public async Task<IActionResult> DeleteAnswer([FromRoute] int id, [FromBody] int usrId)
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
                var result = await answerRepository.DeleteAnswer(id, usrId);

                if (result == "answer deleted successfully")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

        }


        [HttpGet("GetUserAnswers/{usrId}")]

        public async Task<IActionResult> GetUserAnswers([FromRoute] int usrId , [FromQuery] int page = 1, int pageSize = 20)
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

                var result = await answerRepository.GetUserAnswers(usrId,page,pageSize);
                if (result == null && !result.Any())
                {
                    return NotFound("answers not exists");
                }
                return Ok(result);


        }

    }
}
