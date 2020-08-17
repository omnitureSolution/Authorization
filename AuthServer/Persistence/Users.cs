using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthServer.Persistence
{
    public class Users  
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<UserPassword> UserPassword { get; set; } = new HashSet<UserPassword>();
        //public DateTime? MailFailedNotifiedAt { get; set; }
        //public DateTime? LastLoginAt { get; set; }
        //public virtual ICollection<UserSystem> UserSystem { get; set; } = new HashSet<UserSystem>();
        //public virtual ICollection<UserPassword> UserPassword { get; set; } = new HashSet<UserPassword>();
        //public virtual ICollection<UserAccess> UserAccess { get; set; } = new HashSet<UserAccess>();
        //public virtual ICollection<UserToken> UserToken { get; set; } = new HashSet<UserToken>();
        //public Employee Employee { get; set; }
        //public CustomerMaster Customer { get; set; }
        //[NotMapped]
        //public string DisplayName => $"{LastName } {FirstName} {MiddleName}";
    }
}
