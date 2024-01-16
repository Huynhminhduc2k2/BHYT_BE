using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BHYT_BE.Internal.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }  // Quan hệ ngược với User
    }

    public class User : IdentityUser
    {
        [MaxLength(64)]
        public string FullName { get; set; }
        [NotNull]
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
        [Required]
        public ICollection<Role> Roles { get; set; }
    }

    public class EmailDTO
    {
        public string UserEmail { get; set; }
    }

}
