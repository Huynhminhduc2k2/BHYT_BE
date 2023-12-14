using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BHYT_BE.Internal.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        [MaxLength(64)]
        public string Email { get; set; }

        [NotNull]
        [MaxLength(64)]
        public string PasswordHash { get; set; }

        
    }
}
