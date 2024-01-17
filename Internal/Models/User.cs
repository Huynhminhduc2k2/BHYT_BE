using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHYT_BE.Internal.Models
{
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
    }

    public class EmailDTO
    {
        public string UserEmail { get; set; }
    }
}
