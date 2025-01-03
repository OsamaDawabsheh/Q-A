using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAPI_Core.DTOs.Auth
{
    public class GetUsersDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt{ get; set; }
        public string ImgName { get; set; }

        public string role {  get; set; }
    }
}
