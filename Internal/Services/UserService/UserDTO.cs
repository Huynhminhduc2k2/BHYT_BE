﻿namespace BHYT_BE.Internal.Services.UserService
{
    public class UserDTO
    {
        public string ID { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
