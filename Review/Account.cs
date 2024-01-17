using System;
using System.Collections.Generic;

namespace IS220_PROJECT.Models
{
    public partial class Account
    {
        public Account()
        {
            Customers = new HashSet<Customer>();
            Posts = new HashSet<Post>();
            staff = new HashSet<Staff>();
        }

        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public bool Active { get; set; }
        public int? RoleId { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Staff> staff { get; set; }
    }
}
