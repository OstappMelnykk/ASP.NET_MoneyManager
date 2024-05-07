using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int Type { get; set; }
        public string Description { get; set; }

        [Required]
        public decimal Sum { get; set; }
        public DateTime Date { get; set; }

        public int AccountFromId { get; set; }
        public Account AccountFrom { get; set; }

        public int AccountToId { get; set; }
        public Account AccountTo { get; set; }
    }
}
