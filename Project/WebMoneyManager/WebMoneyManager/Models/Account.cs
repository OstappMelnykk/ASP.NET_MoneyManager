using System;
using System.Collections.Generic;

namespace WebMoneyManager.Models;

public partial class Account
{
    public int AccountsId { get; set; }

    public string AccountsTitle { get; set; } = null!;

    public int? FkUsersId { get; set; }

    public virtual User? FkUsers { get; set; }

    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();

    public virtual ICollection<Transaction> TransactionFkAccountsIdFromNavigations { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionFkAccountsIdToNavigations { get; set; } = new List<Transaction>();
}
