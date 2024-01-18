using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHYT_BE.Internal.Models
{
    public class Role
    {
        public const string ADMIN = "ADMIN";
        public const string USER = "USER";
    }
    public class Sex
    {
        public const string MALE = "MALE";
        public const string FEMALE = "FEMALE";
        public const string OTHER = "OTHER";
    }
    public class User : IdentityUser
    {
        [MaxLength(64)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(20)]
        public string PersonID { get; set; }

        [Column(TypeName = "date")]
        public DateTime DOB { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(64)]
        public string Nation { get; set; }

        [MaxLength(64)]
        public string Nationality { get; set; }

        [MaxLength(10)]
        public string Sex { get; set; }

        [MaxLength(9)]
        public string? OTP { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class EmailDTO
    {
        public string UserEmail { get; set; }
    }
}
