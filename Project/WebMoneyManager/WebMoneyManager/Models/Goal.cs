using System;
using System.Collections.Generic;

namespace WebMoneyManager.Models;

public partial class Goal
{
    public int GoalsId { get; set; }

    public string GoalsTitle { get; set; } = null!;

    public string? GoalsDescription { get; set; }

    public decimal GoalsAmounttocollect { get; set; }

    public int? FkAccountsId { get; set; }

    public virtual Account? FkAccounts { get; set; }
}
