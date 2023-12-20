﻿using BHYT_BE.Internal.Models;

namespace BHYT_BE.Internal.Services.UserService
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }
    }
}
