using System;
using System.Collections.Generic;

namespace ExpenseSplitterAPI.Model;

public partial class Expense
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public string Description { get; set; } = null!;

    public decimal Amount { get; set; }

    public int PayerId { get; set; }

    public virtual ICollection<Debt> Debts { get; set; } = new List<Debt>();

    public virtual Group Group { get; set; } = null!;

    public virtual User Payer { get; set; } = null!;
}
