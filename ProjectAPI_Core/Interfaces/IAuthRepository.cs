using ProjectAPI_Core.DTOs.Auth;
using ProjectAPI_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> Register(User user, String password );
        Task<string> Login(string email, String password);

        Task<IEnumerable<GetUsersDto>> GetUsers(int page , int pageSize);
        Task<GetUsersDto> GetUserInformation(int userId);

        Task<string> ChangePassword(string email, string oldPassword, String newPassword);

    }
}
