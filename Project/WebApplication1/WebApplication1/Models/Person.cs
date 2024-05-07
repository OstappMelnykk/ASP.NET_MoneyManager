using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Person : IdentityUser
    {
        [Key]
        public int PersonId { get; set; }

        public byte[]? Photo { get; set; }

        public ICollection<Account> Accounts { get; set; }
    }
}
