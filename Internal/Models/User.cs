using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BHYT_BE.Internal.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }  // Quan hệ ngược với User
    }

    public class User : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public ICollection<Role> Roles { get; set; }
    }
}
