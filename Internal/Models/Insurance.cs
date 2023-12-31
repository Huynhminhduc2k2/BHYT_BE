﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BHYT_BE.Internal.Models
{
    public class Insurance
    {
        public const string ACCEPTED = "ACCEPTED";
        public const string REJECTED = "REJECTED";
        public const string PENDING = "PENDING";
        public const string FEMALE = "FEMALE";
        public const string MALE = "MALE";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceID { get; set; }
        [MaxLength(64)]
        public string FullName { get; set; }
        [NotNull]
        [MaxLength(20)]
        public string PersonID { get; set; }
        [MaxLength(13)]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "date")]
        public DateTime DOB { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
        [MaxLength(64)]
        public string Email { get; set; }
        [MaxLength(64)]
        public string Nation { get; set; }
        [MaxLength(64)]
        public string Nationality { get; set; }
        [MaxLength(10)]
        public string Sex { get; set; }
        [MaxLength(50)] // ACCEPTED, REJECTED, PENDING
        public string Status { get; set; }
    }
}
