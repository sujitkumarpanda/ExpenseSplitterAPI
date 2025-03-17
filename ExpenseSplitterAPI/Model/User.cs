using System;
using System.Collections.Generic;

namespace ExpenseSplitterAPI.Model;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public int? GroupId { get; set; }

    public virtual ICollection<Debt> DebtOwedByUsers { get; set; } = new List<Debt>();

    public virtual ICollection<Debt> DebtOwedToUsers { get; set; } = new List<Debt>();

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual Group? Group { get; set; }

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Payment> PaymentFromUsers { get; set; } = new List<Payment>();

    public virtual ICollection<Payment> PaymentToUsers { get; set; } = new List<Payment>();
}
