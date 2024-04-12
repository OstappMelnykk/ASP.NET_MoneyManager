using System;
using System.Collections.Generic;

namespace WebMoneyManager.Models;

public partial class User
{
    public int UsersId { get; set; }

    public string UsersName { get; set; } = null!;

    public string UsersPhonenumber { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string UsersEmail { get; set; } = null!;

    public byte[]? UsersPhoto { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
