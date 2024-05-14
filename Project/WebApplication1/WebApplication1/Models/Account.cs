using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string PersonId { get; set; }

        public Person Person { get; set; }



       




        public ICollection<Goal>? Goals { get; set; }
        public ICollection<Transaction>? TransactionsFrom { get; set; }
        public ICollection<Transaction>? TransactionsTo { get; set; }
    }
}




/*public string CurrentPersonId { get; set; }

public Person CurrentPerson { get; set; }
*/