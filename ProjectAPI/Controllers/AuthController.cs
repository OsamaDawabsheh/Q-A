using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI_Core.Helper;
using ProjectAPI_Core.DTOs.Auth;
using ProjectAPI_Core.Interfaces;
using ProjectAPI_Core.Models;
using ProjectAPI_Infrastructure.Repositories;
using ProjectAPI.Helper;
using Microsoft.AspNetCore.Authorization;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;
        private readonly UserManager<User> userManager;

        public AuthController(IAuthRepository authRepository, UserManager<User> userManager)
        {
            this.authRepository = authRepository;
            this.userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            registerDto.ImgName =  FilesSettings.UploadFile(registerDto.Img, "Users");


            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                ImgName = registerDto.ImgName,

            };

            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                FilesSettings.DeleteFile(user.ImgName, "Users");


                if (existingUser.EmailConfirmed == false)
                {
                    return BadRequest(new { Message = "User already exists. Please confirm email." });

                }
                return BadRequest(new { Message = "User already exists. Please use another email." });
            }

            var result = await authRepository.Register(user, registerDto.Password);




                await userManager.AddToRoleAsync(user, "User");

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmEmailUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, protocol: Request.Scheme);

                var body = $@"
                        <html>
                                <body>
                         <div style='width: 100%; padding: 20px; display: flex; justify-content: center;'>
                                    <table style='max-width: 400px; border: 3px solid #4CAF50; padding: 20px; border-radius: 50px; text-align: center;'>
                                        <tr>
                                            <td>
                                                <h3 style='color: #4CAF50; font-size: 24px; margin-bottom: 20px;'>Confirm Email</h3>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='pading: 10px 0;'>
                                                <p style='color: #333; font-size: 16px;'>
                                                    If you requested to confirm your email, please click the link below.
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='padding: 20px 0;'>
                                                <a href='{confirmEmailUrl}'' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 20px; display: inline-block;'>
                                                    Confirm Email
                                                </a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='padding: 10px 0;'>
                                                <p style='color: #333; font-size: 14px;'>
                                                    If you didn't request a email confirm, please ignore this email.
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='padding-top: 20px;'>
                                                <p style='color: #666; font-size: 14px;'>
                                                    Best regards,<br>Osama
                                                </p>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                </body>
                            </html>
                        ";

                var email = new EmailDto()
                {
                    Subject = "Confirm Email",
                    Receiver = user.Email,
                    Body = body
                };
                EmailSettings.SendEmail(email);

            return Ok("Registration successful! Please check your email to confirm.");


        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

                var token = await authRepository.Login(loginDto.Email, loginDto.Password);

                if (token == null)
                {
                    return Unauthorized(new { Message = "invalid email or password" });
                }
                return Ok(token);

        }


        [HttpGet("GetUsers")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, int pageSize = 20)
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

                var result = await authRepository.GetUsers(page,pageSize);
                if (result == null && !result.Any())
                {
                    return NotFound("users not exists");
                }
                return Ok(result);
            

        }

        [HttpGet("GetUserInformation/{usrId}")]

        public async Task<IActionResult> GetUserInformation(int usrId)
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

                var result = await authRepository.GetUserInformation(usrId);
                if (result == null)
                {
                    return NotFound("user  not exists");
                }
                return Ok(result);


        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing");
            }


                var userId = ExtractClaims.ExtractUserIdFromToken(token);

                if (!userId.HasValue)
                {
                    return Unauthorized("Invalid user  token.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existUser = await userManager.FindByEmailAsync(changePasswordDto.Email);

                if (existUser != null && existUser.Id != userId)
                {
                    return BadRequest(new{ Message = "you should login with account email that you try to change it password" });
                }

                var result = await authRepository.ChangePassword(changePasswordDto.Email, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

                if (result == "Password changed successfully.")
                {
                    return Ok(result);
                }

                return StatusCode(500, new { Message = "an expected error" });

 
        }



        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Invalid email confirmation request.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully");
   
            }
            else
            {
                return BadRequest("Email confirmation failed.");
            }
        }


        [HttpPost("ForgotPassword")]

        public async Task<string> ForgotPassword([FromForm] string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return "User not found.";
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(token))
            {
                return "Error generating password reset token.";
            }

            var resetPasswordUrl = Url.Action("ResetPassword", "Auth", new { token = token, email = email }, protocol: Request.Scheme);



            var body = $@"
          <html>
                  <body>
           <div style='width: 100%; padding: 20px; display: flex; justify-content: center;'>
                      <table style='max-width: 400px; border: 3px solid #4CAF50; padding: 20px; border-radius: 50px; text-align: center;'>
                          <tr>
                              <td>
                                  <h3 style='color: #4CAF50; font-size: 24px; margin-bottom: 20px;'>Reset Password</h3>
                              </td>
                          </tr>
                          <tr>
                              <td style='pading: 10px 0;'>
                                  <p style='color: #333; font-size: 16px;'>
                                      If you requested to reset your password, please click the link below.
                                  </p>
                              </td>
                          </tr>
                          <tr>
                              <td style='padding: 20px 0;'>
                                  <a href='{resetPasswordUrl}'' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 20px; display: inline-block;'>
                                      Reset Password
                                  </a>
                              </td>
                          </tr>
                          <tr>
                              <td style='padding: 10px 0;'>
                                  <p style='color: #333; font-size: 14px;'>
                                      If you didn't request a email confirm, please ignore this email.
                                  </p>
                              </td>
                          </tr>
                          <tr>
                              <td style='padding-top: 20px;'>
                                  <p style='color: #666; font-size: 14px;'>
                                      Best regards,<br>Osama
                                  </p>
                              </td>
                          </tr>
                      </table>
                  </div>
                  </body>
              </html>
          ";

            var emailDto = new EmailDto()
            {
                Subject = "Password Reset Request",
                Receiver = user.Email,
                Body = body
            };

            EmailSettings.SendEmail(emailDto);

            return "Password reset link has been sent to your email address.";
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto == null || string.IsNullOrEmpty(resetPasswordDto.Token) || string.IsNullOrEmpty(resetPasswordDto.NewPassword))
            {
                return BadRequest("Invalid password reset request.");
            }

            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password has been successfully reset.");
            }

            return BadRequest("Failed to reset password. Please try again.");
        }

    }
}
