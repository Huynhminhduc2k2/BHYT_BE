using BHYT_BE.Internal.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Internal.Services.UserService
{
    public class UserDTO
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PersonID { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public string Nation { get; set; }
        public string Nationality { get; set; }
        public string Sex { get; set; }
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }

    }
}
