using System.ComponentModel.DataAnnotations;

namespace AuthServer.Persistence
{
    public class UserPassword  
    {
        [Key]
        public int UserPasswordId { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
        public virtual Users User { get; set; }
    }
}
