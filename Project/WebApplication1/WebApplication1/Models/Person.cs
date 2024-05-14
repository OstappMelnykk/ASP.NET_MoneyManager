using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class Person : IdentityUser
    {

        public byte[]? Photo { get; set; }

        

        public ICollection<Account> Accounts { get; set; }
    }
}



/*public Account CurrentAccount { get; set; }*/