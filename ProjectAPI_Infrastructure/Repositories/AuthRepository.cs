using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectAPI.Helper;
using ProjectAPI_Core.DTOs.Auth;
using ProjectAPI_Core.Helper;
using ProjectAPI_Core.Interfaces;
using ProjectAPI_Core.Models;
using ProjectAPI_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;



namespace ProjectAPI_Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManger;
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;
        private readonly RoleManager<IdentityRole<int>> roleManager;

        public AuthRepository(UserManager<User> userManager, SignInManager<User> signInManger, IConfiguration configuration,AppDbContext context ,RoleManager<IdentityRole<int>> roleManager)
            {
            this.userManager = userManager;
            this.signInManger = signInManger;
            this.configuration = configuration;
            this.context = context;
            this.roleManager = roleManager;
        }
            public async Task<string> Register(User user, string password)
            {


                var result = await userManager.CreateAsync(user, password);


                if (result.Succeeded)
                {
                return "user registered successfully";
                }



            var errors = result.Errors.Select(error => error.Description).ToList();
                return string.Join(", ", errors);
            }

            public async Task<string> Login(string email, string password)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return "invalid email or password";
                }
                var result = await signInManger.PasswordSignInAsync(user, password, false, false);
                if (!result.Succeeded)
                {
                    return null;
                }
                return GenerateToken(user);
            }


            private string GenerateToken(User user)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:Issuer"],
                    audience: configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

        
        public async Task<IEnumerable<GetUsersDto>> GetUsers(int page , int pageSize)
        {

            var users = context.Users.Select(u=>new GetUsersDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                ImgName = u.ImgName,
                role = context.UserRoles
                                .Where(ur => ur.UserId == u.Id)
                                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                                .FirstOrDefault()
            }).Skip((page - 1) * pageSize).Take(pageSize).Take(pageSize).AsQueryable();

            return (IEnumerable<GetUsersDto>)users;

        }

        public async Task<GetUsersDto> GetUserInformation(int userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) {
                throw new KeyNotFoundException($"user with ID {userId} not found.");
            }

            var getUserInfo = new GetUsersDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                ImgName = user.ImgName,
                role = context.UserRoles
                                .Where(ur => ur.UserId == userId)
                                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                                .FirstOrDefault()
            };

            return getUserInfo;
        }

        public async Task<string> ChangePassword(string email, string oldPassword, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return "User not found.";
            }

            var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
            {
                return "Password changed successfully.";
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return $"Failed to change password: {errors}";
        }


    }
    
}
